import json
from datetime import datetime, timezone
from pathlib import Path

CHECKPOINT_DIR = Path(__file__).resolve().parent.parent / ".checkpoints"


def _path(name: str) -> Path:
    CHECKPOINT_DIR.mkdir(exist_ok=True)
    return CHECKPOINT_DIR / f"{name}.json"


def load(name: str, default: dict) -> dict:
    path = _path(name)
    if not path.exists():
        return dict(default)
    with path.open() as fh:
        return json.load(fh)


def save(name: str, state: dict) -> None:
    state = dict(state)
    state["last_updated"] = datetime.now(timezone.utc).isoformat()
    with _path(name).open("w") as fh:
        json.dump(state, fh, indent=2)
