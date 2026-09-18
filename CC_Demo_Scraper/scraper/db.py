import json
from contextlib import contextmanager

import psycopg
from psycopg import sql
from ulid import ULID

from . import config

TABLE = sql.Identifier(config.SOURCE_TABLE_NAME)


@contextmanager
def get_connection():
    conn = psycopg.connect(
        host=config.PGHOST,
        port=config.PGPORT,
        dbname=config.PGDATABASE,
        user=config.PGUSER,
        password=config.PGPASSWORD,
    )
    try:
        yield conn
    finally:
        conn.close()


def record_exists(conn, resource: str, source_id: int) -> bool:
    with conn.cursor() as cur:
        cur.execute(
            sql.SQL("SELECT 1 FROM {} WHERE resource = %s AND gcd_id = %s LIMIT 1").format(TABLE),
            (resource, source_id),
        )
        return cur.fetchone() is not None


def insert_record(conn, resource: str, source_id: int, data: dict) -> None:
    """Insert one raw row. Caller decides whether to dedupe via record_exists first."""
    with conn.cursor() as cur:
        cur.execute(
            sql.SQL(
                "INSERT INTO {} (id, gcd_id, resource, data, datetime_ingested) VALUES (%s, %s, %s, %s, now())"
            ).format(TABLE),
            (str(ULID()), source_id, resource, json.dumps(data)),
        )
    conn.commit()


def series_by_recency(conn) -> list[dict]:
    """Series already ingested, newest year_began first (nulls last)."""
    with conn.cursor() as cur:
        cur.execute(
            sql.SQL(
                """
                SELECT gcd_id, data
                FROM {}
                WHERE resource = 'series'
                ORDER BY (data->>'year_began')::int DESC NULLS LAST, gcd_id DESC
                """
            ).format(TABLE)
        )
        return [{"source_id": row[0], "data": row[1]} for row in cur.fetchall()]
