import logging

from .. import config, db
from ..checkpoint import load, save
from ..http_client import RateLimitedSession, RequestError
from .base import Source

logger = logging.getLogger("scraper")

SERIES_CHECKPOINT = "series"
ISSUES_CHECKPOINT = "issues"

SERIES_CHECKPOINT_DEFAULT = {
    "next_page_url": f"{config.SOURCE_BASE_URL}/series/",
    "pages_processed": 0,
    "matches_found": 0,
    "failed_pages": [],
}

ISSUES_CHECKPOINT_DEFAULT = {
    "series_done": [],
    "issues_ingested": 0,
    "failed_issue_urls": [],
}


def _matches_filter(series_item: dict) -> bool:
    publisher_url = series_item.get("publisher") or ""
    return publisher_url.rstrip("/").endswith(f"/publisher/{config.SOURCE_PUBLISHER_ID}")


def crawl_series(max_pages: int | None = None) -> None:
    """Phase 1: walk the full series catalog once, keep rows matching the
    configured publisher filter, and checkpoint after every page so a re-run
    resumes from where it left off instead of starting over."""
    state = load(SERIES_CHECKPOINT, SERIES_CHECKPOINT_DEFAULT)
    session = RateLimitedSession()
    pages_this_run = 0
    found_this_run = 0

    with db.get_connection() as conn:
        while state["next_page_url"]:
            if max_pages is not None and pages_this_run >= max_pages:
                logger.info("Reached --max-pages limit (%d); stopping for now.", max_pages)
                break

            url = state["next_page_url"]
            try:
                payload = session.get_json(url)
            except RequestError as exc:
                logger.error("Giving up on page, will retry next run: %s", exc)
                if url not in state["failed_pages"]:
                    state["failed_pages"].append(url)
                save(SERIES_CHECKPOINT, state)
                break

            hits = [item for item in payload["results"] if _matches_filter(item)]
            for item in hits:
                source_id = _id_from_api_url(item["api_url"])
                if not db.record_exists(conn, "series", source_id):
                    db.insert_record(conn, "series", source_id, item)

            pages_this_run += 1
            found_this_run += len(hits)
            state["pages_processed"] += 1
            state["matches_found"] += len(hits)
            state["next_page_url"] = payload.get("next")
            save(SERIES_CHECKPOINT, state)

            logger.info(
                "page %d done: +%d matches this page (%d this run, %d total).",
                state["pages_processed"],
                len(hits),
                found_this_run,
                state["matches_found"],
            )

    if not state["next_page_url"]:
        logger.info("Series catalog fully crawled. %d matches found in total.", state["matches_found"])


def crawl_issues(max_series: int | None = None) -> None:
    """Phase 2: for each ingested series, newest year_began first, fetch and
    store every issue via that series' active_issues list."""
    state = load(ISSUES_CHECKPOINT, ISSUES_CHECKPOINT_DEFAULT)
    session = RateLimitedSession()
    series_processed_this_run = 0

    with db.get_connection() as conn:
        series_list = db.series_by_recency(conn)
        done = set(state["series_done"])
        pending = [s for s in series_list if s["source_id"] not in done]

        if not pending:
            logger.info("No pending series to crawl issues for. Run 'crawl-series' first if you expect more.")
            return

        for series in pending:
            if max_series is not None and series_processed_this_run >= max_series:
                logger.info("Reached --max-series limit (%d); stopping for now.", max_series)
                break

            source_id = series["source_id"]
            issue_urls = series["data"].get("active_issues") or []
            ingested_this_series = 0

            for issue_url in issue_urls:
                issue_id = _id_from_api_url(issue_url)
                if db.record_exists(conn, "issue", issue_id):
                    continue
                try:
                    payload = session.get_json(issue_url)
                except RequestError as exc:
                    logger.error("Failed to fetch issue %s, will retry next run: %s", issue_url, exc)
                    if issue_url not in state["failed_issue_urls"]:
                        state["failed_issue_urls"].append(issue_url)
                    continue
                db.insert_record(conn, "issue", issue_id, payload)
                ingested_this_series += 1
                state["issues_ingested"] += 1

            state["series_done"].append(source_id)
            series_processed_this_run += 1
            save(ISSUES_CHECKPOINT, state)

            logger.info(
                "series %s (%s) done: +%d issues (%d total ingested). %d series remaining.",
                source_id,
                series["data"].get("name", "?"),
                ingested_this_series,
                state["issues_ingested"],
                len(pending) - series_processed_this_run,
            )


def status() -> None:
    series_state = load(SERIES_CHECKPOINT, SERIES_CHECKPOINT_DEFAULT)
    issues_state = load(ISSUES_CHECKPOINT, ISSUES_CHECKPOINT_DEFAULT)

    with db.get_connection() as conn:
        with conn.cursor() as cur:
            cur.execute(db.sql.SQL("SELECT resource, count(*) FROM {} GROUP BY resource").format(db.TABLE))
            counts = dict(cur.fetchall())

    print("Row counts:", counts or "(empty)")
    print(
        f"Series crawl: page {series_state['pages_processed']} done, "
        f"{series_state['matches_found']} matches found, "
        f"{'catalog fully crawled' if not series_state['next_page_url'] else 'in progress'}, "
        f"{len(series_state['failed_pages'])} failed pages."
    )
    print(
        f"Issue crawl: {len(issues_state['series_done'])} series fully processed, "
        f"{issues_state['issues_ingested']} issues ingested, "
        f"{len(issues_state['failed_issue_urls'])} failed issue fetches."
    )


def retry_failed() -> None:
    """Re-attempt anything that previously failed, without re-walking what already succeeded."""
    series_state = load(SERIES_CHECKPOINT, SERIES_CHECKPOINT_DEFAULT)
    issues_state = load(ISSUES_CHECKPOINT, ISSUES_CHECKPOINT_DEFAULT)
    session = RateLimitedSession()

    with db.get_connection() as conn:
        still_failed_pages = []
        for url in series_state["failed_pages"]:
            try:
                payload = session.get_json(url)
            except RequestError as exc:
                logger.error("Still failing: %s (%s)", url, exc)
                still_failed_pages.append(url)
                continue
            for item in (i for i in payload["results"] if _matches_filter(i)):
                source_id = _id_from_api_url(item["api_url"])
                if not db.record_exists(conn, "series", source_id):
                    db.insert_record(conn, "series", source_id, item)
            logger.info("Recovered failed page: %s", url)
        series_state["failed_pages"] = still_failed_pages
        save(SERIES_CHECKPOINT, series_state)

        still_failed_issues = []
        for url in issues_state["failed_issue_urls"]:
            issue_id = _id_from_api_url(url)
            if db.record_exists(conn, "issue", issue_id):
                continue
            try:
                payload = session.get_json(url)
            except RequestError as exc:
                logger.error("Still failing: %s (%s)", url, exc)
                still_failed_issues.append(url)
                continue
            db.insert_record(conn, "issue", issue_id, payload)
            issues_state["issues_ingested"] += 1
            logger.info("Recovered failed issue: %s", url)
        issues_state["failed_issue_urls"] = still_failed_issues
        save(ISSUES_CHECKPOINT, issues_state)


def _id_from_api_url(api_url: str) -> int:
    return int(api_url.rstrip("/").rsplit("/", 1)[-1])


class CatalogSource(Source):
    name = config.SOURCE_NAME

    def commands(self) -> dict:
        return {
            "crawl-series": lambda args: crawl_series(max_pages=args.max_pages),
            "crawl-issues": lambda args: crawl_issues(max_series=args.max_series),
            "status": lambda args: status(),
            "retry-failed": lambda args: retry_failed(),
        }
