import logging
import time

import requests

from . import config

logger = logging.getLogger("scraper")

MAX_RETRIES = 5
BACKOFF_BASE_SECONDS = 2.0
BACKOFF_CAP_SECONDS = 60.0


class RequestError(Exception):
    """Raised when a request fails after all retries."""


class RateLimitedSession:
    """A requests.Session that sleeps between calls and retries on 429/5xx/network errors."""

    def __init__(self):
        self._session = requests.Session()
        self._session.headers["Accept"] = "application/json"
        if config.SOURCE_USERNAME and config.SOURCE_PASSWORD:
            self._session.auth = (config.SOURCE_USERNAME, config.SOURCE_PASSWORD)
        self._last_request_at = 0.0

    def _throttle(self):
        elapsed = time.monotonic() - self._last_request_at
        wait = config.SOURCE_REQUEST_DELAY_SECONDS - elapsed
        if wait > 0:
            time.sleep(wait)

    def get_json(self, url: str, params: dict | None = None) -> dict:
        last_error = None
        for attempt in range(1, MAX_RETRIES + 1):
            self._throttle()
            self._last_request_at = time.monotonic()
            try:
                response = self._session.get(url, params=params, timeout=30)
            except requests.RequestException as exc:
                last_error = exc
                logger.warning("Request error on %s (attempt %d/%d): %s", url, attempt, MAX_RETRIES, exc)
                time.sleep(min(BACKOFF_BASE_SECONDS * (2 ** (attempt - 1)), BACKOFF_CAP_SECONDS))
                continue

            if response.status_code == 429:
                retry_after = float(response.headers.get("Retry-After", 0)) or (
                    BACKOFF_BASE_SECONDS * (2 ** (attempt - 1))
                )
                retry_after = min(retry_after, BACKOFF_CAP_SECONDS)
                logger.warning("429 rate-limited on %s (attempt %d/%d), sleeping %.1fs", url, attempt, MAX_RETRIES, retry_after)
                time.sleep(retry_after)
                continue

            if response.status_code >= 500:
                last_error = requests.RequestException(f"HTTP {response.status_code}")
                logger.warning("Server error %d on %s (attempt %d/%d)", response.status_code, url, attempt, MAX_RETRIES)
                time.sleep(min(BACKOFF_BASE_SECONDS * (2 ** (attempt - 1)), BACKOFF_CAP_SECONDS))
                continue

            if response.status_code == 404:
                raise RequestError(f"404 Not Found: {url}")

            response.raise_for_status()
            return response.json()

        raise RequestError(f"Giving up on {url} after {MAX_RETRIES} attempts: {last_error}")
