<div align="center">

# Majo.Logging

**一个小巧、集中式的 .NET 日志封装。**

基于 Serilog 提供简洁的静态日志 API，支持可配置的文件日志、日志级别、滚动文件、Tag 与异常输出。

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
![Serilog](https://img.shields.io/badge/Serilog-based-2C2D72?style=flat-square)

[English](./README.md) · **简体中文**

</div>

---

## 项目简介

`Majo.Logging` 为 .NET 应用提供一套精简的全局日志 API。

它在 Serilog 之上提供项目自有的封装，使业务代码可以统一通过 `Majo.Logging` 写入日志，而不需要在整个代码库中直接依赖和使用 Serilog API。

当前实现支持：

- 全局静态 Logger；
- 可配置的文件日志；
- 六种日志级别；
- 每条日志独立指定 Tag；
- 异常输出；
- 按日期和文件大小滚动日志；
- 可配置的日志文件保留数量；
- 异步文件写入；
- 初始化前通过 `Debug.WriteLine(...)` 提供回退输出。

## 快速开始

在应用正常记录日志之前初始化一次：

```csharp
using Majo.Logging;

Logger.Initialize();

Logger.Information("Application started.");
Logger.Debug("Debug information.", "Startup");

Logger.Shutdown();
```

`Logger.Shutdown()` 会释放当前 Logger，并将 `Majo.Logging` 恢复为可再次初始化的状态。

## 日志记录

`Logger` 为每一种受支持的日志级别提供一个对应方法：

```csharp
Logger.Verbose("Verbose message.");
Logger.Debug("Debug message.");
Logger.Information("Information message.");
Logger.Warning("Warning message.");
Logger.Error("Error message.");
Logger.Fatal("Fatal message.");
```

当前支持的日志级别：

| Majo.Logging 级别 | 输出文本 |
| --- | --- |
| `Verbose` | `VERBOSE` |
| `Debug` | `DEBUG` |
| `Information` | `INFO` |
| `Warning` | `WARN` |
| `Error` | `ERROR` |
| `Fatal` | `FATAL` |

每个日志方法还可以传入可选的 Tag 和异常：

```csharp
Logger.Error(
    "Failed to process the request.",
    "Network",
    new InvalidOperationException("Example exception"));
```

如果不指定 Tag，则默认使用：

```text
Common
```

## 配置

默认配置无法满足需求时，可以向 `Logger.Initialize(...)` 传入 `LogOption`：

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

| 选项 | 默认值 | 说明 |
| --- | ---: | --- |
| `WriteToFile` | `true` | 是否启用文件日志。 |
| `FilePath` | `null` | 日志文件路径；为 null 或空白时使用默认路径。 |
| `FileLogLevel` | `Debug` | 写入日志文件的最低日志级别。 |
| `FileCountLimit` | `30` | 最多保留的日志文件数量。 |
| `FileSizeLimit` | `10 MiB` | 单个日志文件达到该大小后触发按大小滚动。 |

当 `FilePath` 为 null 或空白时，默认路径为：

```text
<AppContext.BaseDirectory>/log_.log
```

需要的父目录会自动创建。

## 文件输出

日志文件的输出格式为：

```text
[yyyy-MM-dd HH:mm:ss] [LEVEL] [Tag] Message
Exception
```

例如：

```text
[2026-09-26 15:30:00] [INFO] [Common] Application started.
```

文件日志同时采用：

- 按天滚动；
- 达到 `FileSizeLimit` 后按文件大小滚动。

旧日志文件按照 `FileCountLimit` 控制保留数量。

文件写入通过 Serilog 的异步 Sink 完成。

## 初始化与关闭

`Majo.Logging` 使用一个进程级的全局静态 Logger。

```csharp
Logger.Initialize();
```

当 Logger 已经初始化时，再次调用 `Initialize(...)` 不会执行任何操作。

```csharp
Logger.Shutdown();
```

Logger 尚未初始化时调用 `Shutdown()` 同样不会执行任何操作。

调用 `Shutdown()` 后，可以再次调用 `Logger.Initialize(...)` 并使用新的配置重新初始化。

应用在正常退出过程中应关闭 Logger。

## 初始化前的日志

在调用 `Logger.Initialize(...)` 之前记录日志时，不会创建 Serilog Logger，也不会创建日志文件。

此时 `Majo.Logging` 会通过：

```csharp
System.Diagnostics.Debug.WriteLine(...)
```

输出一份回退日志。

例如：

```csharp
Logger.Information("Application is not initialized yet.");
```

`Logger.Shutdown()` 之后、再次初始化之前，同样采用这一行为。

## 依赖

当前实现基于：

- `Serilog`
- `Serilog.Sinks.Async`
- `Serilog.Sinks.File`

这些依赖被封装在 `Majo.Logging` 内部。通常情况下，调用方只需要使用 `Logger`、`LogOption` 和 `LogLevel`。

## 设计原则

`Majo.Logging` 刻意保持较小的公共 API：

- 为应用提供一个统一的全局日志入口；
- 保持常用日志调用简洁；
- 将 Serilog 相关配置集中在日志库内部；
- 只暴露当前项目真正需要的配置；
- 优先解决具体需求，不为假想场景提前增加抽象。

## 许可证

参阅 [LICENSE.txt](./LICENSE.txt)。

---

<div align="center">

作为 Majo 系列项目中一个专注的日志组件。

[English](./README.md)

</div>
