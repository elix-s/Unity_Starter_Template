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
    private LogLevel _minimumLogLevel = LogLevel.Info;
    
    public void Log(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0
        )
    {
        if (_minimumLogLevel > LogLevel.Info)
            return;

        string formattedMessage = FormatMessage("LOG", message, callerFile,  callerName, callerLine);
        UnityEngine.Debug.Log(formattedMessage);
    }
    
    public void LogWarning(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0
        )
    {
        if (_minimumLogLevel > LogLevel.Warning)
            return;

        string formattedMessage = FormatMessage("WARNING", message, callerFile, callerName, callerLine);
        UnityEngine.Debug.LogWarning(formattedMessage);
    }
    
    public void LogError(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (_minimumLogLevel > LogLevel.Error)
            return;

        string formattedMessage = FormatMessage("ERROR", message, callerFile, callerName, callerLine);
        UnityEngine.Debug.LogError(formattedMessage);
    }
    
    private string FormatMessage(string logType, string message, string callerFile, string callerName, int callerLine)
    {
        return $"[{logType}] [{DateTime.Now:HH:mm:ss}] [{callerFile} : {callerName} : {callerLine}] {message}";
    }
}
