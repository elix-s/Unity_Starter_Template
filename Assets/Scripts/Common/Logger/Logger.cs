using System.Diagnostics;

public class Logger
{
    public void Log(string message)
    {
        var stackTrace = new StackTrace();
        var callerFrame = stackTrace.GetFrame(1);
        var callerMethod = callerFrame.GetMethod();
        var callerClass = callerMethod.DeclaringType;
        
        UnityEngine.Debug.Log($"[LOG] [{callerClass?.Name}]: {message}");
    }
}
