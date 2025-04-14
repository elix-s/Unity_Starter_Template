using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;

public enum LogLevel
{
    Info,
    Warning,
    Error
}

public class Logger
{
    private LogLevel _minimumLogLevel = LogLevel.Info;
    private Queue<string> _logQueue = new Queue<string>();

    [Conditional("DEVELOPMENT_BUILD")]
    public void Log(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (_minimumLogLevel > LogLevel.Info)
            return;

        string formattedMessage = FormatMessage("LOG", message, callerFile, callerName, callerLine);
        _logQueue.Enqueue(formattedMessage);
        UnityEngine.Debug.Log(formattedMessage);
    }

    [Conditional("DEVELOPMENT_BUILD")]
    public void LogWarning(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (_minimumLogLevel > LogLevel.Warning)
            return;

        string formattedMessage = FormatMessage("WARNING", message, callerFile, callerName, callerLine);
        _logQueue.Enqueue(formattedMessage);
        UnityEngine.Debug.LogWarning(formattedMessage);
    }

    [Conditional("DEVELOPMENT_BUILD")]
    public void LogError(string message,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (_minimumLogLevel > LogLevel.Error)
            return;

        string formattedMessage = FormatMessage("ERROR", message, callerFile, callerName, callerLine);
        _logQueue.Enqueue(formattedMessage);
        UnityEngine.Debug.LogError(formattedMessage);
    }
    
    [Conditional("DEVELOPMENT_BUILD")]
    public void LogException(Exception exception,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerFile = "",
        [CallerLineNumber] int callerLine = 0)
    {
        if (_minimumLogLevel > LogLevel.Error)
            return;
        
        string combinedMessage = $"{exception.Message}{Environment.NewLine}{exception.StackTrace}";
        string formattedMessage = FormatMessage("EXCEPTION", combinedMessage, callerFile, callerName, callerLine);
        _logQueue.Enqueue(formattedMessage);
        UnityEngine.Debug.LogError(formattedMessage);
    }

    private string FormatMessage(string logType, string message, string callerFile, string callerName, int callerLine)
    {
        string fileName = Path.GetFileName(callerFile);
        return $"[{logType}] [{DateTime.Now:HH:mm:ss}] [{fileName} : {callerName} : {callerLine}] {message}";
    }
    
    public void ExportLogsToFile(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        var logs = string.Join(Environment.NewLine, _logQueue.ToArray());
        File.WriteAllText(filePath, logs);
    }
}
