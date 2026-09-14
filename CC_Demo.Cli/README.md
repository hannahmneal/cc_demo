# cc_demo CLI

A small maintenance CLI for the CC_Demo database.

## Running

Rewrites the copyright year in every `© <year> MARVEL` string across given columns.

Flow:

1. **Pick tables.** You get a lettered menu of the permitted tables plus an
   `ALL TABLES` shortcut; enter one letter, several (`a,b`), or the ALL letters.
2. It shows the copyright year(s) currently in the allowed tables.
3. It asks what the year should become — must be 4 digits, anything else
   re-prompts.
4. You confirm. All tables are updated in a **single transaction** (one
   `UPDATE` per table), so the batch is all-or-nothing.

```
dotnet run --project CC_Demo.Cli -- run copyright tool
```

Piped input is supported for scripting:

```
printf '2026\ny\n' | dotnet run --project CC_Demo.Cli -- run copyright tool
```

## Platform support

Works on macOS, Windows and Linux (needs the .NET 9 SDK).
