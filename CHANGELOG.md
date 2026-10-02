# Changelog

All notable changes to `Majo.Logging` are documented in this file.

## 0.0.3 - 2026-10-02

### Added

- Added validation for log options.

## 0.0.2 - 2026-10-01

### Added

- Added `LogEntry` to expose structured log entry information.
- Added `Logger.LogWritten` for observing successfully written log entries.

### Changed

- `Logger.Initialize(...)` now returns whether the logger was initialized by the current call, allowing callers to track logger ownership.
- Made logger initialization, shutdown, and writes thread-safe.
- Isolated exceptions thrown by `LogWritten` subscribers so they do not interrupt logging.
- Marked Serilog packages as implementation-only dependencies so their compile-time APIs are no longer exposed to package consumers.

### Fixed

- Log messages are now written through a fixed Serilog message template so message content is treated as data rather than as a message template.

## 0.0.1 - 2026-09-26

- Initial release.