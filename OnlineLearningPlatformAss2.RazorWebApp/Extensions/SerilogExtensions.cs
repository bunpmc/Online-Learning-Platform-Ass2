using Serilog.Core;

namespace OnlineLearningPlatformAss2.RazorWebApp.Extensions;

/// <summary>
/// Extension methods for Serilog configuration
/// </summary>
public static class SerilogExtensions
{
  /// <summary>
  /// Gets the logger for a specific class
  /// </summary>
  public static Serilog.ILogger GetLogger(Type type)
  {
    return Log.ForContext(type);
  }

  /// <summary>
  /// Gets the logger for a specific class with generic syntax
  /// </summary>
  public static Serilog.ILogger GetLogger<T>()
  {
    return Log.ForContext<T>();
  }

  /// <summary>
  /// Logs an exception with additional context
  /// </summary>
  public static void LogException(this Serilog.ILogger logger, Exception ex, string message, params object[] args)
  {
    logger.Error(ex, message, args);
  }
}
