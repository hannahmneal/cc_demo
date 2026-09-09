using CC_Demo.Models;
using CC_Demo.Models.Marvel;
using CC_Demo.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Tests.TestSupport;

/// <summary>
/// A real <see cref="AppDbContext"/> backed by a private in-memory SQLite database.
/// Exercises the actual EF model - value converters, owned collections, key mapping -
/// rather than an EF-InMemory fake that would silently skip all of that.
/// </summary>
public sealed class SqliteFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Db { get; }

    public SqliteFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new AppDbContext(options);
        Db.Database.EnsureCreated();

        // The `cc_demo` table is mapped with ExcludeFromMigrations() because it is managed
        // outside EF in production - which also makes EnsureCreated() skip it. Create it by
        // hand so MarvelRecord-backed code is testable.
        Db.Database.ExecuteSqlRaw(
            """
            CREATE TABLE IF NOT EXISTS cc_demo (
                id TEXT NOT NULL PRIMARY KEY,
                marvel_id INTEGER NOT NULL,
                attribution_html TEXT NOT NULL,
                attribution_text TEXT NOT NULL,
                copyright TEXT NOT NULL,
                data TEXT NOT NULL,
                resource TEXT NOT NULL,
                resource_uri TEXT NOT NULL
            );
            """);
    }

    /// <summary>A fresh context over the same database, so a test can read back what it wrote without stale tracking.</summary>
    public AppDbContext NewContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);

    public static Creator NewCreator(int marvelId, string fullName, Ulid? id = null) => new()
    {
        Id = id ?? Ulid.NewUlid(),
        MarvelId = marvelId,
        FirstName = fullName.Split(' ').First(),
        LastName = fullName.Split(' ').Last(),
        FullName = fullName,
        AttributionHtml = "<a href=\"http://marvel.com\">Data provided by Marvel. © 2024 MARVEL</a>",
        AttributionText = "Data provided by Marvel. © 2024 MARVEL",
        Copyright = "© 2024 MARVEL",
        Resource = "creators",
    };

    public static MarvelRecord NewRecord(string resource, int marvelId, string data = "{}", Ulid? id = null) => new()
    {
        Id = id ?? Ulid.NewUlid(),
        MarvelId = marvelId,
        Resource = resource,
        Data = data,
        AttributionHtml = "<a href=\"http://marvel.com\">Data provided by Marvel. © 2024 MARVEL</a>",
        AttributionText = "Data provided by Marvel. © 2024 MARVEL",
        Copyright = "© 2024 MARVEL",
        ResourceUri = $"http://gateway.marvel.com/v1/public/{resource}/{marvelId}",
    };

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
