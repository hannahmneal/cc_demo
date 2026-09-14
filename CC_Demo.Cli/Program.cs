using System.Text.RegularExpressions;
using Npgsql;
using Spectre.Console;

// Entry point for the `cc_demo` tool.
//
//   cc_demo run copyright tool
//
// Lets you pick choose tables from a list, shows the copyright year currently 
// stored in selected columns and rewrites every "© <year> MARVEL" string to 
// a year you provide.

var connectionString = Environment.GetEnvironmentVariable("PG_CONNECTION");
if (string.IsNullOrWhiteSpace(connectionString))
{
    AnsiConsole.MarkupLine("[red]PG_CONNECTION environment variable is not set.[/]");
    return 1;
}

var expectedArgs = new[] { "run", "copyright", "tool" };
if (!args.Select(a => a.ToLowerInvariant()).SequenceEqual(expectedArgs))
{
    AnsiConsole.MarkupLine("[yellow]Usage:[/] cc_demo run copyright tool");
    return 1;
}

// The columns that carry the "© <year> MARVEL" string.
string[] columns = ["AttributionHtml", "AttributionText", "Copyright"];

// The tables that may be altered. 
// ❗️Note: If modifying this list use the exact Postgres table name — 
// identifiers are case-sensitive because the schema uses quoted 
// PascalCase names (e.g., "Creators", not "creators"/"CREATORS").
string[] tables = ["Creators"];

// Matches "© 2024 MARVEL" and tolerates odd spacing/casing.
var yearPattern = new Regex(@"©\s*(?<year>\d{4})\s*MARVEL", RegexOptions.IgnoreCase);

if (tables.Length == 0)
{
    AnsiConsole.MarkupLine("[red]No tables are configured for this tool.[/]");
    return 1;
}

var selectedTables = PromptForTables(tables);

await using var conn = new NpgsqlConnection(connectionString);
try
{
    await conn.OpenAsync();
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Could not connect to the database:[/] {ex.Message}");
    return 1;
}

// Make sure each selected table exists and has all desired columns.
var unusable = new List<string>();
foreach (var table in selectedTables)
    if (!await TableIsUpdatable(table))
        unusable.Add(table);

if (unusable.Count > 0)
{
    AnsiConsole.MarkupLine(
        $"[red]Unable to update (missing table, or missing one of {string.Join(", ", columns)}):[/] " +
        string.Join(", ", unusable));
    return 1;
}

// Collect every distinct "© <year> MARVEL" fragment across the selected tables.
var fragments = new HashSet<string>();
var columnList = string.Join(", ", columns.Select(c => $"\"{c}\""));

foreach (var table in selectedTables)
{
    await using var cmd = new NpgsqlCommand($"SELECT {columnList} FROM \"{table}\"", conn);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        for (var i = 0; i < columns.Length; i++)
        {
            if (reader.IsDBNull(i)) continue;
            foreach (Match m in yearPattern.Matches(reader.GetString(i)))
                fragments.Add(m.Value);
        }
    }
}

if (fragments.Count == 0)
{
    AnsiConsole.MarkupLine(
        $"[yellow]No '© <year> MARVEL' strings found in {string.Join(", ", selectedTables)}. Nothing to do.[/]");
    return 0;
}

var currentYears = fragments
    .Select(f => yearPattern.Match(f).Groups["year"].Value)
    .Distinct()
    .OrderBy(y => y)
    .ToList();

AnsiConsole.MarkupLine(
    $"Current copyright year{(currentYears.Count == 1 ? "" : "s")} in " +
    $"{string.Join(", ", selectedTables)}: [green]{string.Join(", ", currentYears)}[/]");

var newYear = PromptForYear();

var replacements = fragments
    .Select(f => new { Old = f, Year = yearPattern.Match(f).Groups["year"].Value })
    .Where(x => x.Year != newYear)
    .Select(x => new { x.Old, New = x.Old.Replace(x.Year, newYear) })
    .ToList();

if (replacements.Count == 0)
{
    AnsiConsole.MarkupLine($"[yellow]Those tables already use {newYear}. Nothing to do.[/]");
    return 0;
}

var summary = string.Join(", ", replacements.Select(r => $"\"{r.Old}\" -> \"{r.New}\""));
if (!Confirm($"Update {summary} across {string.Join(", ", columns)} in {string.Join(", ", selectedTables)}?"))
{
    AnsiConsole.MarkupLine("[yellow]Aborted. No changes made.[/]");
    return 0;
}

// One UPDATE per table: each column gets a REPLACE() chained over every
// (old -> new) pair, and the WHERE keeps us from rewriting untouched rows.
// The whole batch runs in a single transaction, so it is all-or-nothing.
string ColumnExpression(string column)
{
    var expr = $"\"{column}\"";
    for (var i = 0; i < replacements.Count; i++)
        expr = $"REPLACE({expr}, @old{i}, @new{i})";
    return expr;
}

var setClause = string.Join(", ", columns.Select(c => $"\"{c}\" = {ColumnExpression(c)}"));
var whereClause = string.Join(" OR ",
    from c in columns
    from i in Enumerable.Range(0, replacements.Count)
    select $"\"{c}\" LIKE '%' || @old{i} || '%'");

var rowsByTable = new Dictionary<string, int>();
await using (var tx = await conn.BeginTransactionAsync())
{
    foreach (var table in selectedTables)
    {
        await using var cmd = new NpgsqlCommand(
            $"UPDATE \"{table}\" SET {setClause} WHERE {whereClause}", conn, tx);
        for (var i = 0; i < replacements.Count; i++)
        {
            cmd.Parameters.AddWithValue($"old{i}", replacements[i].Old);
            cmd.Parameters.AddWithValue($"new{i}", replacements[i].New);
        }
        rowsByTable[table] = await cmd.ExecuteNonQueryAsync();
    }

    await tx.CommitAsync();
}

AnsiConsole.MarkupLine($"[green]Done.[/] Copyright year is now [green]{newYear}[/].");
foreach (var (table, rows) in rowsByTable)
    AnsiConsole.MarkupLine($"  {table}: {rows} row(s) updated");
return 0;

// ---------------------------------------------------------------------------
// Prompt helpers (work at a real terminal and with piped stdin)
// ---------------------------------------------------------------------------
string[] PromptForTables(string[] permitted)
{
    // One lettered option per table, plus a trailing "ALL TABLES" shortcut.
    var labels = permitted.Select(t => t.ToUpperInvariant()).Append("ALL TABLES").ToArray();
    var allIndex = labels.Length - 1;

    while (true)
    {
        AnsiConsole.MarkupLine("Please select which table(s) you wish to update:");
        for (var i = 0; i < labels.Length; i++)
            AnsiConsole.MarkupLine($"  ([green]{(char)('a' + i)}[/]) {labels[i]}");
        AnsiConsole.Markup("Enter one or more letters (e.g. [grey]a[/] or [grey]a,b[/]): ");

        var line = Console.ReadLine();
        if (line is null)
            throw new InvalidOperationException("No input provided for the table selection.");

        var tokens = line.Split([',', ' '],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Select at least one option.[/]");
            continue;
        }

        var indices = new List<int>();
        string? invalid = null;
        foreach (var token in tokens)
        {
            var c = char.ToLowerInvariant(token[0]);
            if (token.Length != 1 || c < 'a' || c >= 'a' + labels.Length)
            {
                invalid = token;
                break;
            }
            indices.Add(c - 'a');
        }

        if (invalid is not null)
        {
            AnsiConsole.MarkupLine($"[red]'{invalid}' is not one of the options.[/]");
            continue;
        }

        return indices.Contains(allIndex)
            ? permitted
            : indices.Distinct().Select(i => permitted[i]).ToArray();
    }
}

string PromptForYear()
{
    const string question = "What should the copyright year be changed to?";
    var yearOnly = new Regex(@"^\d{4}$");

    if (!Console.IsInputRedirected)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(question)
                .Validate(i => yearOnly.IsMatch(i.Trim())
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Enter a 4-digit year (e.g. 2026).[/]")))
            .Trim();
    }

    while (true)
    {
        AnsiConsole.Markup($"{question} ");
        var line = Console.ReadLine();
        if (line is null) throw new InvalidOperationException("No input provided for the copyright year.");
        if (yearOnly.IsMatch(line.Trim())) return line.Trim();
        AnsiConsole.MarkupLine("[red]Enter a 4-digit year (e.g. 2026).[/]");
    }
}

bool Confirm(string prompt)
{
    if (!Console.IsInputRedirected) return AnsiConsole.Confirm(prompt);

    AnsiConsole.Markup($"{prompt} [grey](y/n)[/] ");
    var line = Console.ReadLine();
    return line is not null && line.Trim().StartsWith("y", StringComparison.OrdinalIgnoreCase);
}

async Task<bool> TableIsUpdatable(string table)
{
    const string sql = """
        SELECT COUNT(*)
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = @table
          AND column_name = ANY(@columns)
        """;

    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("table", table);
    cmd.Parameters.AddWithValue("columns", columns);
    return Convert.ToInt32(await cmd.ExecuteScalarAsync()) == columns.Length;
}
