using StardewModdingAPI;
using StardewValley;

namespace weizinai.StardewValleyMod.Common;

internal static class Logger
{
    private static IMonitor monitor = null!;

    public static void Init(IMonitor _monitor)
    {
        monitor = _monitor;
    }

    #region SMAPI控制台日志

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

    #endregion

    public static void NoIconHUDMessage(string message, float timeLeft = 3500f)
    {
        Game1.addHUDMessage(new HUDMessage(message, timeLeft) { noIcon = true });
    }

    public static void ErrorHUDMessage(string message, float timeLeft = 3500f)
    {
        Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = timeLeft });
    }
}