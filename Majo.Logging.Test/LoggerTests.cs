using Xunit;

namespace Majo.Logging.Test;

/// <summary>
/// Logger test class
/// </summary>
public sealed class LoggerTests : IDisposable
{
    /// <summary>
    /// Temporary directory for test files
    /// </summary>
    private readonly string _tempDirectory;
    
    /// <summary>
    /// List of files to delete after tests
    /// </summary>
    private readonly List<string> _filesToDelete = [];

    /// <summary>
    /// Constructor
    /// </summary>
    public LoggerTests()
    {
        Logger.Shutdown();

        _tempDirectory = Path.Combine(Path.GetTempPath(), "Majo.Logging.Test", Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Dispose method to clean up resources
    /// </summary>
    public void Dispose()
    {
        Logger.Shutdown();

        foreach (string file in _filesToDelete)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    /// <summary>
    /// Test that custom file path writes expected log levels to the log file
    /// </summary>
    [Fact]
    public void CustomFilePath_WritesExpectedLogLevels()
    {
        string filePath = Path.Combine(_tempDirectory, "custom", "log_.log");

        Logger.Initialize(new LogOption
        {
            WriteToFile = true,
            FilePath = filePath,
            FileLogLevel = LogLevel.Debug
        });

        Logger.Verbose("This verbose log should not be written to the file.", "Custom");
        Logger.Debug("Debug test.", "Custom");
        Logger.Information("Information test.", "Custom");
        Logger.Warning("Warning test.", "Custom");
        Logger.Error("Error test.", "Custom", new InvalidOperationException("Test error exception"));
        Logger.Fatal("Fatal test.", "Custom", new InvalidOperationException("Test fatal exception"));

        Logger.Shutdown();

        string directory = Path.GetDirectoryName(filePath)!;

        string logFile = Assert.Single(Directory.GetFiles(directory, "log_*.log"));

        string content = File.ReadAllText(logFile);

        Assert.DoesNotContain("This verbose log should not be written to the file.", content);

        Assert.Contains("[DEBUG] [Custom] Debug test.", content);

        Assert.Contains("[INFO] [Custom] Information test.", content);

        Assert.Contains("[WARN] [Custom] Warning test.", content);

        Assert.Contains("[ERROR] [Custom] Error test.", content);

        Assert.Contains("System.InvalidOperationException: Test error exception", content);

        Assert.Contains("[FATAL] [Custom] Fatal test.", content);
        
        Assert.Contains("System.InvalidOperationException: Test fatal exception", content);
    }

    /// <summary>
    /// Test that default file path writes to the application directory
    /// </summary>
    [Fact]
    public void DefaultFilePath_WritesToApplicationDirectory()
    {
        string marker = $"Default path test {Guid.NewGuid():N}";

        Logger.Initialize();

        Logger.Information(marker);

        Logger.Shutdown();

        string[] matchingFiles = Directory.GetFiles(AppContext.BaseDirectory, "log_*.log")
            .Where(file => File.ReadAllText(file).Contains(marker, StringComparison.Ordinal)).ToArray();

        _filesToDelete.AddRange(matchingFiles);

        string logFile = Assert.Single(matchingFiles);
        
        string content = File.ReadAllText(logFile);
        
        Assert.Contains($"[INFO] [Common] {marker}", content);
    }

    /// <summary>
    /// Test that when file logging is disabled, no log file is created
    /// </summary>
    [Fact]
    public void FileLoggingDisabled_DoesNotWriteLogFile()
    {
        string filePath = Path.Combine(_tempDirectory, "disabled", "log_.log");

        Logger.Initialize(new LogOption
        {
            WriteToFile = false,
            FilePath = filePath
        });

        Logger.Information("This log should not create a file.", "NoFile");

        Logger.Shutdown();

        string directory = Path.GetDirectoryName(filePath)!;

        string[] files = Directory.Exists(directory) ? Directory.GetFiles(directory) : [];

        Assert.Empty(files);
    }
}