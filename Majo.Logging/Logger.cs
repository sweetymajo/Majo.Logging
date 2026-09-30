using Serilog;

namespace Majo.Logging;

/// <summary>
/// Provides a global logger for writing application log entries
/// </summary>
public static class Logger
{
    /// <summary>
    /// Occurs when a log entry is written
    /// </summary>
    public static event Action<LogEntry>? LogWritten;
    
    /// <summary>
    /// Locks access to the logger state to ensure thread safety
    /// </summary>
    private static readonly ReaderWriterLockSlim StateLock = new();
    
    /// <summary>
    /// The active Serilog logger
    /// </summary>
    private static Serilog.Core.Logger? _logger;
    
    /// <summary>
    /// The default log file name
    /// </summary>
    private const string DefaultFileName = "log_.log";
    
    /// <summary>
    /// The template used to format file log entries
    /// </summary>
    private const string OutputTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{LogLevel}] [{LogTag}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// The default tag assigned to log entries
    /// </summary>
    private const string DefaultTag = "Common";
    
    /// <summary>
    /// Initializes the logger with the specified options or the defaults
    /// </summary>
    /// <param name="option">The logging options, or <see langword="null"/> to use the defaults</param>
    /// <returns>
    /// <see langword="true"/> if the logger was initialized; <see langword="false"/> if it was already initialized
    /// </returns>
    public static bool Initialize(LogOption? option = null)
    {
        StateLock.EnterWriteLock();
        
        try
        {
            if (_logger is not null)
            {
                return false; // Already initialized
            }
            
            option ??= new LogOption();
        
            // Create the logging configuration
            var configuration = new LoggerConfiguration().MinimumLevel.Verbose()
                .Enrich.With(new LogEnricher());
            // Apply the file logging options
            ConfigureFile(configuration, option);
            // Create the logger
            _logger = configuration.CreateLogger();
        
            return true;
        }
        finally
        {
            StateLock.ExitWriteLock();
        }


    }
    
    /// <summary>
    /// Writes a verbose log entry
    /// </summary>
    /// <param name="content">The message to log</param>
    /// <param name="tag">The tag to associate with the entry</param>
    /// <param name="exception">The exception to include, if any</param>
    public static void Verbose(string content, string tag = DefaultTag, Exception? exception = null)
        => Write(LogLevel.Verbose, tag, content, exception);
    
    /// <summary>
    /// Writes a debug log entry
    /// </summary>
    /// <param name="content">The message to log</param>
    /// <param name="tag">The tag to associate with the entry</param>
    /// <param name="exception">The exception to include, if any</param>
    public static void Debug(string content, string tag = DefaultTag, Exception? exception = null)
        => Write(LogLevel.Debug, tag, content, exception);
    
    /// <summary>
    /// Writes an informational log entry
    /// </summary>
    /// <param name="content">The message to log</param>
    /// <param name="tag">The tag to associate with the entry</param>
    /// <param name="exception">The exception to include, if any</param>
    public static void Information(string content, string tag = DefaultTag, Exception? exception = null)
        => Write(LogLevel.Information, tag, content, exception);
    
    /// <summary>
    /// Writes a warning log entry
    /// </summary>
    /// <param name="content">The message to log</param>
    /// <param name="tag">The tag to associate with the entry</param>
    /// <param name="exception">The exception to include, if any</param>
    public static void Warning(string content, string tag = DefaultTag, Exception? exception = null)
        => Write(LogLevel.Warning, tag, content, exception);
    
    /// <summary>
    /// Writes an error log entry
    /// </summary>
    /// <param name="content">The message to log</param>
    /// <param name="tag">The tag to associate with the entry</param>
    /// <param name="exception">The exception to include, if any</param>
    public static void Error(string content, string tag = DefaultTag, Exception? exception = null)
        => Write(LogLevel.Error, tag, content, exception);
    
    /// <summary>
    /// Writes a fatal log entry
    /// </summary>
    /// <param name="content">The message to log</param>
    /// <param name="tag">The tag to associate with the entry</param>
    /// <param name="exception">The exception to include, if any</param>
    public static void Fatal(string content, string tag = DefaultTag, Exception? exception = null)
        => Write(LogLevel.Fatal, tag, content, exception);

    /// <summary>
    /// Closes the logger and releases its resources
    /// </summary>
    public static void Shutdown()
    {
        StateLock.EnterWriteLock();
        
        try
        {
            if (_logger is null)
            {
                return; // Not initialized
            }
            
            Serilog.Core.Logger logger = _logger;
            _logger = null;
            
            logger.Dispose();
        }
        finally
        {
            StateLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Configures file output when it is enabled in the logging options
    /// </summary>
    /// <param name="configuration">The Serilog configuration to update</param>
    /// <param name="option">The file logging options to apply</param>
    private static void ConfigureFile(LoggerConfiguration configuration, LogOption option)
    {
        if (!option.WriteToFile)
        {
            return;
        }
        
        string filePath = string.IsNullOrWhiteSpace(option.FilePath) ? 
            Path.Combine(AppContext.BaseDirectory, DefaultFileName) : Path.GetFullPath(option.FilePath);

        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory))
        {
            // Ensure the log directory exists
            Directory.CreateDirectory(directory);
        }

        configuration.WriteTo.Async(writeTo =>
            writeTo.File(
                filePath,
                restrictedToMinimumLevel: LogUtils.ToSerilogLevel(option.FileLogLevel),
                outputTemplate: OutputTemplate,
                shared: true,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: option.FileSizeLimit,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: option.FileCountLimit
            ));
    }

    /// <summary>
    /// Writes a log entry or sends it to debug output before initialization
    /// </summary>
    /// <param name="level">The severity of the entry</param>
    /// <param name="tag">The tag to associate with the entry</param>
    /// <param name="content">The message to log</param>
    /// <param name="exception">The exception to include, if any</param>
    private static void Write(LogLevel level, string tag, string content, Exception? exception)
    {
        LogEntry entry = new(DateTime.Now, level, tag, content, exception);
        
        bool written = false;
        
        StateLock.EnterReadLock();
        
        try
        {
            if (_logger is not null)
            {
                _logger.ForContext("LogTag", tag).Write(LogUtils.ToSerilogLevel(level), exception, 
                    "{Content}", content);
                written = true;
            }
        }
        finally
        {
            StateLock.ExitReadLock();
        }
        
        if (!written)
        {
            // Use debug output when the logger has not been initialized
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                                               $"[{LogUtils.ToLevelText(level)}] " +
                                               $"[{tag}] {content}");

            if (exception is not null)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
            
            return;
        }
        
        Publish(entry);
    }

    /// <summary>
    /// Publishes a log entry to all subscribed handlers
    /// </summary>
    /// <param name="entry">The log entry to publish</param>
    private static void Publish(LogEntry entry)
    {
        Action<LogEntry>? handlers = LogWritten;

        if (handlers is null)
        {
            return;
        }

        foreach (var handler in handlers.GetInvocationList())
        {
            if (handler is not Action<LogEntry> action)
            {
                continue;
            }
            
            try
            {
                action(entry);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in log handler: {e}");
            }
        }
    }
}