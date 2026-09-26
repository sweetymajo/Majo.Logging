<div align="center">

# Majo.Logging

**A small, centralized logging wrapper for .NET applications.**

A simple static logging API built on Serilog, with configurable file logging, log levels, rolling files, tags, and exception output.

[![NuGet](https://img.shields.io/nuget/v/Majo.Logging?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/Majo.Logging)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
![Serilog](https://img.shields.io/badge/Serilog-based-2C2D72?style=flat-square)

**English** · [简体中文](./README.zh-CN.md)

</div>

---

## Overview

`Majo.Logging` provides a small global logging API for .NET applications.

It wraps Serilog behind a compact, project-owned interface so application code can write logs without depending directly on Serilog APIs throughout the codebase.

The current implementation supports:

- a global static logger;
- configurable file logging;
- six log levels;
- per-message tags;
- exception output;
- daily and size-based log rolling;
- configurable file retention;
- asynchronous file writes;
- fallback to `Debug.WriteLine(...)` before initialization.

## Installation

`Majo.Logging` is available on [NuGet.org](https://www.nuget.org/packages/Majo.Logging):

```bash
dotnet add package Majo.Logging

## Quick Start

Initialize the logger once before normal application logging:

```csharp
using Majo.Logging;

Logger.Initialize();

Logger.Information("Application started.");
Logger.Debug("Debug information.", "Startup");

Logger.Shutdown();
```

`Logger.Shutdown()` disposes the active logger and resets `Majo.Logging` so it can be initialized again later.

## Logging

`Logger` exposes one method for each supported level:

```csharp
Logger.Verbose("Verbose message.");
Logger.Debug("Debug message.");
Logger.Information("Information message.");
Logger.Warning("Warning message.");
Logger.Error("Error message.");
Logger.Fatal("Fatal message.");
```

The available levels are:

| Majo.Logging level | Output text |
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

If no tag is supplied, the default tag is:

```text
Common
```

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

### `LogOption`

| Option | Default | Description |
| --- | ---: | --- |
| `WriteToFile` | `true` | Enables or disables file logging. |
| `FilePath` | `null` | File path used for log output. When null or whitespace, the default path is used. |
| `FileLogLevel` | `Debug` | Minimum level written to the log file. |
| `FileCountLimit` | `30` | Maximum number of retained log files. |
| `FileSizeLimit` | `10 MiB` | Maximum size of a log file before size-based rolling. |

When `FilePath` is null or whitespace, the default path is:

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
[2026-09-26 15:30:00] [INFO] [Common] Application started.
```

File logging uses both:

- daily rolling;
- rolling when `FileSizeLimit` is reached.

Old files are retained according to `FileCountLimit`.

File output is written through Serilog's asynchronous sink.

## Initialization and Shutdown

`Majo.Logging` uses a single process-wide static logger.

```csharp
Logger.Initialize();
```

Calling `Initialize(...)` again while the logger is already initialized has no effect.

```csharp
Logger.Shutdown();
```

Calling `Shutdown()` when the logger is not initialized also has no effect.

After shutdown, `Logger.Initialize(...)` can be called again with a new configuration.

Applications should shut the logger down during normal application termination.

## Logging Before Initialization

Calls made before `Logger.Initialize(...)` do not create a Serilog logger or a log file.

Instead, `Majo.Logging` writes a fallback representation through:

```csharp
System.Diagnostics.Debug.WriteLine(...)
```

For example:

```csharp
Logger.Information("Application is not initialized yet.");
```

This behavior also applies after `Logger.Shutdown()` until the logger is initialized again.

## Dependencies

The current implementation is built on:

- `Serilog`
- `Serilog.Sinks.Async`
- `Serilog.Sinks.File`

These dependencies are kept behind the `Majo.Logging` API so callers normally only need to interact with `Logger`, `LogOption`, and `LogLevel`.

## Design Goals

`Majo.Logging` deliberately keeps its public API small:

- provide one application-wide logging entry point;
- keep common logging calls concise;
- keep Serilog-specific setup inside the logging library;
- expose only configuration that the current projects actually need;
- prefer simple, concrete behavior over unnecessary abstraction.

## License

See [LICENSE.txt](./LICENSE.txt).

---

<div align="center">

Built as a focused logging component for the Majo project family.

[简体中文](./README.zh-CN.md)

</div>
