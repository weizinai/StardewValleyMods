using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>
/// 按模组泛型闭包隔离的 SMAPI 控制台日志工具。
/// 泛型参数 T 为各模组的 <c>ModEntry</c> 标记类型，编译器保证每个模组一份独立静态状态，
/// 使日志经各模组自己的 monitor 发出，SMAPI 控制台来源名保持显示各模组名。
/// </summary>
/// <typeparam name="T">模组的 <c>ModEntry</c> 类型。</typeparam>
public static class Logger<T>
{
    /// <summary>
    /// 本闭包内该模组自己的 monitor。
    /// </summary>
    private static IMonitor? monitor;

    /// <summary>
    /// 初始化日志：登记模组 monitor，供本闭包后续日志调用使用。
    /// </summary>
    /// <param name="mod">模组入口实例。</param>
    public static void Init(Mod mod)
    {
        monitor = mod.Monitor;
        HudLogger.Register(mod.ModManifest.UniqueID, mod.Monitor);
    }

    /// <summary>
    /// 输出 Trace 级别日志。
    /// </summary>
    /// <param name="message">日志内容。</param>
    public static void Trace(string message)
    {
        GetMonitor()?.Log(message);
    }

    /// <summary>
    /// 输出 Debug 级别日志。
    /// </summary>
    /// <param name="message">日志内容。</param>
    public static void Debug(string message)
    {
        GetMonitor()?.Log(message, LogLevel.Debug);
    }

    /// <summary>
    /// 输出 Info 级别日志。
    /// </summary>
    /// <param name="message">日志内容。</param>
    public static void Info(string message)
    {
        GetMonitor()?.Log(message, LogLevel.Info);
    }

    /// <summary>
    /// 输出 Warn 级别日志。
    /// </summary>
    /// <param name="message">日志内容。</param>
    public static void Warn(string message)
    {
        GetMonitor()?.Log(message, LogLevel.Warn);
    }

    /// <summary>
    /// 输出 Error 级别日志。
    /// </summary>
    /// <param name="message">日志内容。</param>
    public static void Error(string message)
    {
        GetMonitor()?.Log(message, LogLevel.Error);
    }

    /// <summary>
    /// 输出 Alert 级别日志。
    /// </summary>
    /// <param name="message">日志内容。</param>
    public static void Alert(string message)
    {
        GetMonitor()?.Log(message, LogLevel.Alert);
    }

    /// <summary>
    /// 获取本闭包 monitor；未初始化时回退到全局兜底 monitor，避免空引用。
    /// </summary>
    /// <returns>可用的 monitor；均未注册时返回 null，日志安全丢弃。</returns>
    private static IMonitor? GetMonitor()
    {
        return monitor ?? HudLogger.FallbackMonitor;
    }
}