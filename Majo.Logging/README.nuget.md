# Majo.Logging

A small, centralized logging wrapper for .NET applications.

`Majo.Logging` provides a compact static logging API built on Serilog, with configurable file logging, log levels, tags, exception output, log rolling, and asynchronous file writes.

## Installation

```bash
dotnet add package Majo.Logging
```

The package targets .NET 10.

## Quick Start

Initialize the logger once before normal application logging:

```csharp
using Majo.Logging;

Logger.Initialize();

Logger.Information("Application started.");
Logger.Debug("Debug information.", "Startup");

Logger.Shutdown();
```

`Logger.Shutdown()` disposes the active logger and flushes the underlying logging pipeline.

## Logging

`Logger` exposes one method for each supported log level:

```csharp
Logger.Verbose("Verbose message.");
Logger.Debug("Debug message.");
Logger.Information("Information message.");
Logger.Warning("Warning message.");
Logger.Error("Error message.");
Logger.Fatal("Fatal message.");
```

The available levels are:

| Level | Output text |
| --- | --- |
| `Verbose` | `VERBOSE` |
| `Debug` | `DEBUG` |
| `Information` | `INFO` |
| `Warning` | `WARN` |
| `Error` | `ERROR` |
| `Fatal` | `FATAL` |

Each logging method also accepts an optional tag and exception:

```csharp
Logger.Error(
    "Failed to process the request.",
    "Network",
    new InvalidOperationException("Example exception"));
```

If no tag is supplied, the default tag is `Common`.

## Configuration

Pass a `LogOption` to `Logger.Initialize(...)` when the defaults are not sufficient:

```csharp
Logger.Initialize(new LogOption
{
    WriteToFile = true,
    FilePath = "logs/log_.log",
    FileLogLevel = LogLevel.Information,
    FileCountLimit = 14,
    FileSizeLimit = 5L * 1024 * 1024
});
```

| Option | Default | Description |
| --- | ---: | --- |
| `WriteToFile` | `true` | Enables or disables file logging. |
| `FilePath` | `null` | File path used for log output. When null or whitespace, the default path is used. |
| `FileLogLevel` | `Debug` | Minimum level written to the log file. |
| `FileCountLimit` | `30` | Maximum number of retained log files. |
| `FileSizeLimit` | `10 MiB` | Maximum file size before size-based rolling. |

When `FilePath` is null or whitespace, logs are written under:

```text
<AppContext.BaseDirectory>/log_.log
```

Parent directories are created automatically when necessary.

## File Output

The file output format is:

```text
[yyyy-MM-dd HH:mm:ss] [LEVEL] [Tag] Message
Exception
```

For example:

```text
[2026-09-26 17:00:00] [INFO] [Common] Application started.
```

File logging uses:

- daily rolling;
- size-based rolling using `FileSizeLimit`;
- retention using `FileCountLimit`;
- asynchronous writes through Serilog's async sink.

## Initialization and Shutdown

`Majo.Logging` uses a single process-wide static logger.

Calling `Logger.Initialize(...)` again while the logger is already initialized has no effect.

Calling `Logger.Shutdown()` when the logger is not initialized also has no effect.

After shutdown, the logger can be initialized again with a new configuration.

Applications should call `Logger.Shutdown()` during normal termination so pending asynchronous log output can be completed.

## Logging Before Initialization

Calls made before `Logger.Initialize(...)`, or after `Logger.Shutdown()`, do not create a log file.

Instead, `Majo.Logging` writes a fallback representation through `System.Diagnostics.Debug.WriteLine(...)`.

## Dependencies

`Majo.Logging` is built on:

- Serilog
- Serilog.Sinks.Async
- Serilog.Sinks.File

These dependencies are kept behind the `Majo.Logging` API so application code normally only needs to interact with `Logger`, `LogOption`, and `LogLevel`.
