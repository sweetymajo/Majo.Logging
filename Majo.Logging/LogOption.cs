namespace Majo.Logging;

/// <summary>
/// Defines the options for file logging
/// </summary>
public class LogOption
{
    /// <summary>
    /// Gets or sets whether log entries are written to a file
    /// </summary>
    public bool WriteToFile { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the path to the log file
    /// </summary>
    public string? FilePath { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum level written to the log file
    /// </summary>
    public LogLevel FileLogLevel { get; set; } = LogLevel.Debug;
    
    /// <summary>
    /// Gets or sets the maximum number of log files to retain
    /// </summary>
    public int FileCountLimit { get; set; } = 30;
    
    /// <summary>
    /// Gets or sets the maximum size of each log file, in bytes
    /// </summary>
    public long FileSizeLimit { get; set; } = 10L * 1024 * 1024;
}