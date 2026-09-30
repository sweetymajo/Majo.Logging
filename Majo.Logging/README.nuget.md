# Majo.Logging

A small, centralized logging wrapper for .NET applications.

`Majo.Logging` provides a compact static logging API built on Serilog, with configurable file logging, log levels, tags, exception output, log rolling, and asynchronous file writes.

## Installation

```bash
dotnet add package Majo.Logging
```

The package targets .NET8 and .NET 10.

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

## Observing Log Entries

`Logger.LogWritten` is raised after a log entry is successfully written through the active logger.

```csharp
Logger.LogWritten += entry =>
{
    Console.WriteLine(
        $"[{entry.LevelText}] [{entry.Tag}] {entry.Content}");
};
```

Each event provides a `LogEntry` containing:

| Property | Description |
| --- | --- |
| `Timestamp` | Time at which the log entry was created. |
| `Level` | The `LogLevel` of the entry. |
| `LevelText` | Display text for the log level, such as `INFO` or `ERROR`. |
| `Tag` | The tag associated with the entry. |
| `Content` | The original log message. |
| `Exception` | The associated exception, if any. |

`LogWritten` is only raised after a log entry is successfully written through the initialized logger. Calls made before initialization or after `Logger.Shutdown()` use the `Debug.WriteLine(...)` fallback and do not raise the event.

Exceptions thrown by individual `LogWritten` handlers are isolated and do not interrupt logging or prevent other subscribers from receiving the entry.

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

```csharp
bool initialized = Logger.Initialize();
```

`Logger.Initialize(...)` returns:

- `true` when the current call initialized the logger;
- `false` when the logger was already initialized.

Calling `Initialize(...)` while the logger is already active does not replace or reconfigure the existing logger.

The return value can be used to track ownership when a component may share the global logger with the rest of the application:

```csharp
bool ownsLogger = Logger.Initialize();

try
{
    // Use the logger.
}
finally
{
    if (ownsLogger)
    {
        Logger.Shutdown();
    }
}
```

`Logger.Shutdown()` releases the current logger. Calling it while the logger is not initialized has no effect.

After shutdown, `Logger.Initialize(...)` can be called again with a new configuration.

Applications or components that own the logger should call `Logger.Shutdown()` during normal termination.

## Logging Before Initialization

Calls made before `Logger.Initialize(...)`, or after `Logger.Shutdown()`, do not create a log file.

Instead, `Majo.Logging` writes a fallback representation through `System.Diagnostics.Debug.WriteLine(...)`.

## Dependencies

`Majo.Logging` is built on:

- Serilog
- Serilog.Sinks.Async
- Serilog.Sinks.File

These dependencies are kept behind the `Majo.Logging` API so callers normally only need to interact with `Logger`, `LogEntry`, `LogOption`, and `LogLevel`.
