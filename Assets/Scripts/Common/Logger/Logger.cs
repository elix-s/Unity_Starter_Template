using System;
using System.Runtime.CompilerServices;

public enum LogLevel
{
    Info,
    Warning,
    Error
}

public class Logger
{
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Info;
    
    public void Log(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (MinimumLogLevel > LogLevel.Info)
            return;

        string formattedMessage = FormatMessage("LOG", message, callerName, callerLine);
        UnityEngine.Debug.Log(formattedMessage);
    }
    
    public void LogWarning(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (MinimumLogLevel > LogLevel.Warning)
            return;

        string formattedMessage = FormatMessage("WARNING", message, callerName, callerLine);
        UnityEngine.Debug.LogWarning(formattedMessage);
    }
    
    public void LogError(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (MinimumLogLevel > LogLevel.Error)
            return;

        string formattedMessage = FormatMessage("ERROR", message, callerName, callerLine);
        UnityEngine.Debug.LogError(formattedMessage);
    }
    
    private string FormatMessage(string logType, string message, string callerName, int callerLine)
    {
        return $"[{logType}] [{DateTime.Now:HH:mm:ss}] [{callerName}:{callerLine}] {message}";
    }
}
