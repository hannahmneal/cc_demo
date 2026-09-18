import argparse
import logging
import sys
from pathlib import Path

from scraper.sources.catalog import CatalogSource

SOURCE = CatalogSource()

LOG_DIR = Path(__file__).resolve().parent / "logs"


def configure_logging() -> None:
    LOG_DIR.mkdir(exist_ok=True)
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s %(levelname)s %(message)s",
        handlers=[
            logging.StreamHandler(sys.stdout),
            logging.FileHandler(LOG_DIR / "scraper.log"),
        ],
    )


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Raw-data scraper")
    commands = parser.add_subparsers(dest="command", required=True)

    crawl_series = commands.add_parser("crawl-series", help="Walk the series catalog, keeping filtered hits")
    crawl_series.add_argument("--max-pages", type=int, default=None, help="Stop after N pages this run")

    crawl_issues = commands.add_parser("crawl-issues", help="Fetch issues for already-ingested series")
    crawl_issues.add_argument("--max-series", type=int, default=None, help="Stop after N series this run")

    commands.add_parser("status", help="Show crawl progress and row counts")
    commands.add_parser("retry-failed", help="Retry pages/issues that previously failed")

    return parser


def main() -> None:
    configure_logging()
    parser = build_parser()
    args = parser.parse_args()

    command = SOURCE.commands()[args.command]
    command(args)


if __name__ == "__main__":
    main()
