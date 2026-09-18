from abc import ABC, abstractmethod


class Source(ABC):
    """A pluggable data source. Each source owns its own raw table, checkpoint
    namespace, and crawl commands -- the CLI just dispatches into one of these."""

    name: str

    @abstractmethod
    def commands(self) -> dict:
        """Map of {command_name: callable(args)} this source exposes to the CLI."""
        raise NotImplementedError
