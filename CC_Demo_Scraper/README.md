# CC_Demo_Scraper

A standalone Python scraper that fills a raw landing table for the [CC_Demo](../README.md) app.
It's decoupled from the C# app entirely -- the database is the only integration point. The raw
table is owned by EF Core migrations on the C# side even though only this scraper writes rows
into it. Nothing here evaluates or normalizes the data; it lands as raw JSON for you to inspect
before deciding what's worth promoting into a normalized table.

**Which external catalog this crawls, which publisher it filters to, and which table it writes
into are deliberately not named anywhere in this code or its history** -- they're all `.env`
values (see `.env.example`). `.env` is gitignored; keep the real values there, not in comments,
commit messages, or this file.

## Setup

```bash
cd CC_Demo_Scraper
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
cp .env.example .env   # then fill in real values; .env is gitignored
```

An optional source-account username/password in `.env` raises the target API's hourly rate limit
above its anonymous tier -- worth setting if `crawl-series` is going to run for hours.

### Local Postgres

From the repo root (`CC_Demo/`), not this directory:

```bash
docker compose up -d      # start
docker compose down       # stop, keeps data
docker compose down -v    # nuke everything including data, for a clean re-scrape
docker exec -it cc_demo_postgres_local psql -U cc_demo_admin -d cc_demo_db   # poke at the data directly
```

The raw table itself is created by the C# app's EF migrations, not by this scraper -- either run
`dotnet ef database update` from `CC_Demo/`, or just start the API once against this same
connection string (it migrates on startup). This scraper only ever `INSERT`s into it.

## Running it

```bash
python main.py crawl-series   # phase 1: find every series matching the configured publisher filter
python main.py crawl-issues   # phase 2: fetch every issue for series found so far
python main.py status         # progress + row counts, safe to run anytime
python main.py retry-failed   # re-attempt anything that errored out in phase 1/2
```

### Why two phases

The source API has no server-side "filter series by publisher" option, so `crawl-series` has to
walk its *entire* series catalog once, keeping only the ones whose `publisher` field matches
`SOURCE_PUBLISHER_ID`. That's slow -- expect it to run for hours -- so it's checkpointed after
*every page* (`.checkpoints/series.json`) and safe to `Ctrl+C` and resume later; it picks up from
the exact next page rather than restarting.

`crawl-issues` reads whatever series are already in the raw table, newest `year_began` first, and
fetches each one's issues. It's checkpointed per series it fully finishes
(`.checkpoints/issues.json`), and every single insert is also guarded by a DB existence check on
`(resource, gcd_id)` -- so even without the checkpoint, re-running never creates duplicate rows.
You don't have to wait for `crawl-series` to finish before starting `crawl-issues`: run it again
later to pick up series discovered in the meantime.

Both commands print a line after every page/series with what just happened -- records added this
page/series, running totals -- and both accept a limit for a bounded test run instead of leaving
it going indefinitely:

```bash
python main.py crawl-series --max-pages 5
python main.py crawl-issues --max-series 10
```

### Errors

Individual page/issue failures (network errors, 429s, 5xxs) are retried with backoff in-process;
if a page or issue still fails after that, it's logged and recorded in the checkpoint's
`failed_pages` / `failed_issue_urls` list rather than aborting the whole run. `python main.py
retry-failed` re-attempts just those. Everything is also logged to `logs/scraper.log`.

## What's in the raw table

| column               | notes                                                                 |
|----------------------|------------------------------------------------------------------------|
| `id`                 | ULID (text), generated here -- there's no DB-side default              |
| `gcd_id`             | the source's own numeric id for the series/issue                       |
| `resource`           | `"series"` or `"issue"`                                                |
| `data`               | the raw JSON payload from the source API, verbatim                     |
| `datetime_ingested`  | when this scraper wrote the row                                        |

`resource = "series"` rows include `active_issues` (the URLs `crawl-issues` walks) and
`year_began`/`year_ended`. `resource = "issue"` rows include a `story_set` array -- that's where
per-issue credit text fields live.

## Adding another source later

Each source gets its own module under `scraper/sources/` implementing the `Source` interface in
`scraper/sources/base.py`, its own raw table + migration on the C# side, and its own `.env`
values. The CLI, checkpointing, and HTTP retry/rate-limit plumbing in `scraper/` are all reusable
as-is.

## Known limitations (the source API, not this scraper)

- Unofficial: its own docs say the API's fields "should not be considered stable."
- No publisher filter, no ordering param that actually works, fixed page size -- hence the
  two-phase design above.
- No dedicated creator/person endpoint. Creators only exist as free-text credit fields per story.
