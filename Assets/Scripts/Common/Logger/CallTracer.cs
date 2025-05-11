using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Diagnostics; 
using System.Runtime.CompilerServices; 
using System.Collections.Generic; 

//class for logging method calls
public static class CallTracer
{
    private static string _logDirectoryPath;
    private static string _stackTraceLogFilePath;
    private static readonly object _listLock = new object(); 
    private static readonly object _fileLock = new object(); 
    private static bool _hasLoggedToFile = false; 

    private static List<LogEntry> _callHistory = new List<LogEntry>();

    private struct LogEntry
    {
        public DateTime Timestamp { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public string Message { get; } 
        public LogEntryType Type { get; }

        public LogEntry(DateTime timestamp, string className, string methodName, LogEntryType type, string message = null)
        {
            Timestamp = timestamp;
            ClassName = className;
            MethodName = methodName;
            Type = type;
            Message = message;
        }

        public override string ToString()
        {
            string entryTypeStr = $"[{Type.ToString().ToUpper()}]";
            string baseInfo = $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {entryTypeStr} {ClassName}.{MethodName}";
            
            if (!string.IsNullOrEmpty(Message))
            {
                baseInfo += $"\n    Details: {Message.Replace("\n", "\n    ")}";
            }
            
            return baseInfo;
        }
    }

    private enum LogEntryType
    {
        MethodCall,
        ExceptionCaught,
        UnhandledException,
        ApplicationQuit
    }

    static CallTracer()
    {
        _logDirectoryPath = Path.Combine(Application.dataPath, "..", "Logs");
        _stackTraceLogFilePath = Path.Combine(_logDirectoryPath, "StackTraceLogs.txt");
        
        Application.logMessageReceivedThreaded += HandleUnityLog;
        Application.quitting += OnApplicationQuitting;
        
        lock (_listLock)
        {
            _callHistory.Add(new LogEntry(DateTime.Now, "CallTracer", "SessionStart", LogEntryType.MethodCall, $"Logging session started. Unity Version: {Application.unityVersion}, Platform: {Application.platform}"));
        }
        
        UnityEngine.Debug.Log("CallTracer initialized. Log will be written on exit or exception.");
    }

    /// <summary>
    /// Register the entry to the method.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)] 
    public static void TraceMethodEntry()
    {
        StackTrace stackTrace = new StackTrace(1, false); 
        StackFrame frame = stackTrace.GetFrame(0);

        string className = "UnknownClass";
        string methodName = "UnknownMethod";

        if (frame != null)
        {
            var methodBase = frame.GetMethod();
            if (methodBase != null)
            {
                className = methodBase.DeclaringType != null ? methodBase.DeclaringType.FullName : "Global";
                methodName = methodBase.Name;
            }
        }

        lock (_listLock)
        {
            _callHistory.Add(new LogEntry(DateTime.Now, className, methodName, LogEntryType.MethodCall));
        }
    }

    /// <summary>
    /// Logs the caught exception. Call from a catch block.
    /// </summary>
    public static void LogHandledException(Exception ex,
                                   [CallerMemberName] string memberName = "",
                                   [CallerFilePath] string sourceFilePath = "",
                                   [CallerLineNumber] int sourceLineNumber = 0)
    {
        string className = "UnknownClass";
       
        if (!string.IsNullOrEmpty(sourceFilePath))
        {
            try { className = Path.GetFileNameWithoutExtension(sourceFilePath); }
            catch { /* ignore */ }
        }
        
        string exceptionDetails = $"In Method: {className}.{memberName} (at {Path.GetFileName(sourceFilePath)}:{sourceLineNumber})\n" +
                                  $"Type: {ex.GetType().Name}\n" +
                                  $"Message: {ex.Message}\n" +
                                  $"StackTrace:\n{ex.StackTrace}";
        
        lock (_listLock)
        {
            _callHistory.Add(new LogEntry(DateTime.Now, className, memberName, LogEntryType.ExceptionCaught, exceptionDetails));
        }
        
        UnityEngine.Debug.LogError($"CallTracer: Handled exception logged: {ex.Message} in {className}.{memberName}");
        DumpLogToFile("ExceptionOccurred"); 
    }

    private static void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception) 
        {
            string exceptionDetails = $"Message: {logString}\n" +
                                      $"StackTrace (from Unity):\n{stackTrace}";
            lock (_listLock)
            {
                string className = "UnknownClass";
                string methodName = "UnknownMethod";
                
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    var firstLine = stackTrace.Split('\n')[0];
                    var parts = firstLine.Split(new[] { ':' }, 2); 
                    if (parts.Length > 0)
                    {
                        var methodFullPath = parts[0].Trim();
                        var lastDot = methodFullPath.LastIndexOf('.');
                        if (lastDot > 0)
                        {
                            className = methodFullPath.Substring(0, lastDot);
                            methodName = methodFullPath.Substring(lastDot + 1);
                        } else {
                            methodName = methodFullPath; 
                        }
                    }
                }
                
                _callHistory.Add(new LogEntry(DateTime.Now, className, methodName, LogEntryType.UnhandledException, exceptionDetails));
            }
            
            UnityEngine.Debug.Log("CallTracer: Unhandled exception detected. Dumping call history.");
            DumpLogToFile("UnhandledException");
        }
    }

    private static void OnApplicationQuitting()
    {
        lock (_listLock)
        {
            _callHistory.Add(new LogEntry(DateTime.Now, "Application", "Quitting", LogEntryType.ApplicationQuit, "Application is quitting."));
        }
        
        UnityEngine.Debug.Log("CallTracer: Application quitting. Dumping call history.");
        DumpLogToFile("ApplicationQuit");
    }

    private static void DumpLogToFile(string reason)
    {
        lock (_fileLock) 
        {
            if (_hasLoggedToFile && reason != "ExceptionOccurred" && reason != "UnhandledException")
            {
                UnityEngine.Debug.Log($"CallTracer: Log already written due to a previous event. Skipping for '{reason}'.");
                return;
            }

            if (_callHistory.Count == 0) 
            {
                 UnityEngine.Debug.Log("CallTracer: Call history is empty. No log to write.");
                return;
            }

            try
            {
                if (!Directory.Exists(_logDirectoryPath))
                {
                    Directory.CreateDirectory(_logDirectoryPath);
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Log dump reason: {reason}");
                sb.AppendLine($"Dumped at: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine("--------------------------------------------------");

                List<LogEntry> historyCopy;
                lock (_listLock)
                {
                    historyCopy = new List<LogEntry>(_callHistory); 
                }

                foreach (var entry in historyCopy)
                {
                    sb.AppendLine(entry.ToString());
                    sb.AppendLine("---"); 
                }
                sb.AppendLine("--------------------------------------------------");
                sb.AppendLine("End of log.");

                File.WriteAllText(_stackTraceLogFilePath, sb.ToString(), Encoding.UTF8);
                UnityEngine.Debug.Log($"CallTracer: Call history dumped to {_stackTraceLogFilePath}");
                _hasLoggedToFile = true; 
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"CallTracer: Failed to dump call history: {ex.Message}");
            }
        }
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/CallTracer/Clear Log File and Reset State")]
    public static void ClearLogFileMenu()
    {
        lock (_fileLock)
        {
            if (File.Exists(_stackTraceLogFilePath))
            {
                try
                {
                    File.Delete(_stackTraceLogFilePath);
                    UnityEngine.Debug.Log($"CallTracer: Log file {_stackTraceLogFilePath} deleted.");
                }
                catch (Exception ex)
                {
                     UnityEngine.Debug.LogError($"CallTracer: Could not delete log file: {ex.Message}");
                }
            }
            
            _hasLoggedToFile = false;
        }
        lock (_listLock)
        {
            _callHistory.Clear();
            _callHistory.Add(new LogEntry(DateTime.Now, "CallTracer", "SessionReset", LogEntryType.MethodCall, "Log state reset via editor."));
        }
        
        UnityEngine.Debug.Log("CallTracer: State reset and log file cleared (if existed).");
    }
    #endif
}
