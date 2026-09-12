using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Multiplayer;

namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>多人消息接收侧：把对端发来的控制台日志按发送方归属输出到发送方自己的 monitor。</summary>
internal static class LogReceiver
{
    private static readonly Dictionary<string, LogLevel> MessageLevels = new()
    {
        [MessageTypes.Info] = LogLevel.Info,
        [MessageTypes.Alert] = LogLevel.Alert
    };

    /// <summary>本机没装发送方模组（注册表里查不到）时静默丢弃，不打出归属不明的日志行。</summary>
    internal static void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (!MessageLevels.TryGetValue(e.Type, out var level)) return;

        var monitor = MonitorRegistry.Get(e.FromModID);

        if (monitor is null) return;

        var message = e.ReadAs<LogMessageData>();

        monitor.Log(message.Content, level);
    }
}
