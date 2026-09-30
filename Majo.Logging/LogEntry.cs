namespace Majo.Logging;

/// <summary>
/// Represents a log entry
/// </summary>
/// <param name="Timestamp">The timestamp of the log entry</param>
/// <param name="Level">The log level of the log entry</param>
/// <param name="Tag">The tag of the log entry</param>
/// <param name="Content">The content of the log entry</param>
/// <param name="Exception">The exception associated with the log entry, if any</param>
public record LogEntry(DateTime Timestamp, LogLevel Level, string Tag, string Content, Exception? Exception = null)
{
    /// <summary>
    /// Gets the text representation of the log level
    /// </summary>
    public string LevelText => LogUtils.ToLevelText(Level);
}