using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>
/// 按模组泛型闭包隔离的 SMAPI 控制台日志工具：<c>T</c> 传调用方自己的 <c>ModEntry</c>，日志便经该模组自己的 monitor 发出，控制台来源名保持各模组名；六个等级与 SMAPI <see cref="LogLevel" /> 同名一一对应。
/// 调用方必须在 <c>Entry</c> 中先调用 <see cref="Init" />——未初始化时日志直接丢弃，不会归到 PiCore 名下。
/// </summary>
public static class Logger<T>
{
    private static IMonitor? monitor;

    public static void Init(Mod mod)
    {
        monitor = mod.Monitor;
        MonitorRegistry.Register(mod.ModManifest.UniqueID, mod.Monitor);
    }

    public static void Trace(string message)
    {
        monitor?.Log(message);
    }

    public static void Debug(string message)
    {
        monitor?.Log(message, LogLevel.Debug);
    }

    public static void Info(string message)
    {
        monitor?.Log(message, LogLevel.Info);
    }

    public static void Warn(string message)
    {
        monitor?.Log(message, LogLevel.Warn);
    }

    public static void Error(string message)
    {
        monitor?.Log(message, LogLevel.Error);
    }

    public static void Alert(string message)
    {
        monitor?.Log(message, LogLevel.Alert);
    }
}
