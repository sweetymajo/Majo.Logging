using Serilog.Core;
using Serilog.Events;

namespace Majo.Logging;

/// <summary>
/// Adds a display level to each Serilog event
/// </summary>
internal sealed class LogEnricher : ILogEventEnricher
{
    /// <summary>
    /// Adds the display level property to a log event
    /// </summary>
    /// <param name="logEvent">The log event to enrich</param>
    /// <param name="propertyFactory">The factory used to create the level property</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // Add the display level property
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("LogLevel", 
            LogUtils.ToLevelText(logEvent.Level)));
    }
}