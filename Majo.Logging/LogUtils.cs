using Serilog.Events;

namespace Majo.Logging;

/// <summary>
/// Provides conversions between application and Serilog log levels
/// </summary>
internal static class LogUtils
{
    /// <summary>
    /// Converts an application log level to a Serilog event level
    /// </summary>
    /// <param name="level">The application log level to convert</param>
    /// <returns>The corresponding Serilog event level</returns>
    internal static LogEventLevel ToSerilogLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Verbose => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Fatal => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
    
    /// <summary>
    /// Converts an application log level to its display text
    /// </summary>
    /// <param name="level">The application log level to convert</param>
    /// <returns>The display text for the log level</returns>
    internal static string ToLevelText(LogLevel level)
    {
        return level switch
        {
            LogLevel.Verbose => "VERBOSE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Fatal => "FATAL",
            _ => "UNKNOWN"
        };
    }
    
    /// <summary>
    /// Converts a Serilog event level to its display text
    /// </summary>
    /// <param name="level">The Serilog event level to convert</param>
    /// <returns>The display text for the event level</returns>
    internal static string ToLevelText(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => "VERBOSE",
            LogEventLevel.Debug => "DEBUG",
            LogEventLevel.Information => "INFO",
            LogEventLevel.Warning => "WARN",
            LogEventLevel.Error => "ERROR",
            LogEventLevel.Fatal => "FATAL",
            _ => "UNKNOWN"
        };
    }
}