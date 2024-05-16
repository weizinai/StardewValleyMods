using StardewModdingAPI;

namespace Common;

internal class Log
{
    private static IMonitor monitor = null!;

    public Log(IMonitor monitor)
    {
        Log.monitor = monitor;
    }

    public static void Trace(string message)
    {
        monitor.Log(message);
    }

    public static void Debug(string message)
    {
        monitor.Log(message, LogLevel.Debug);
    }

    public static void Info(string message)
    {
        monitor.Log(message, LogLevel.Info);
    }

    public static void Warn(string message)
    {
        monitor.Log(message, LogLevel.Warn);
    }
    
    public static void Error(string message)
    {
        monitor.Log(message, LogLevel.Error);
    }
    
    public static void Alert(string message)
    {
        monitor.Log(message, LogLevel.Alert);
    }
}