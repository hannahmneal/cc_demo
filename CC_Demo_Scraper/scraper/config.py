import os

from dotenv import load_dotenv

load_dotenv()


def _env(name: str, default: str | None = None) -> str | None:
    return os.environ.get(name, default)


PGHOST = _env("PGHOST", "localhost")
PGPORT = int(_env("PGPORT", "5434"))
PGDATABASE = _env("PGDATABASE", "cc_demo_db")
PGUSER = _env("PGUSER", "cc_demo_admin")
PGPASSWORD = _env("PGPASSWORD", "")

SOURCE_NAME = _env("SOURCE_NAME", "source")
SOURCE_BASE_URL = _env("SOURCE_BASE_URL", "https://example.invalid/api").rstrip("/")
SOURCE_USERNAME = _env("SOURCE_USERNAME") or None
SOURCE_PASSWORD = _env("SOURCE_PASSWORD") or None
SOURCE_REQUEST_DELAY_SECONDS = float(_env("SOURCE_REQUEST_DELAY_SECONDS", "5.0"))
SOURCE_PUBLISHER_ID = int(_env("SOURCE_PUBLISHER_ID", "0"))
SOURCE_TABLE_NAME = _env("SOURCE_TABLE_NAME", "Raw_Source_Data")