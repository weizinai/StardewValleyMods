using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>
///     多人消息接收侧：统一处理 ModMessageReceived 事件，按发送方模组路由日志与 HUD 提示。
/// </summary>
public static class HudBroadcaster
{
    /// <summary>
    ///     支持处理的消息类型。
    /// </summary>
    private static readonly HashSet<string> DetectedMessageType = new() { "Info", "Alert", "NoIconHUDMessage" };

    /// <summary>
    ///     处理多人消息：按发送方模组 uniqueId 查共享注册表，用该模组本地 monitor 显示，
    ///     对端未安装发送方模组时静默丢弃。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">多人消息事件参数。</param>
    internal static void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (!DetectedMessageType.Contains(e.Type))
        {
            return;
        }

        var monitor = HudLogger.GetMonitor(e.FromModID);

        if (monitor is null)
        {
            return;
        }

        var message = e.ReadAs<MessageData>();

        switch (e.Type)
        {
            case "Info":
                monitor.Log(message.Content, LogLevel.Info);

                break;
            case "Alert":
                monitor.Log(message.Content, LogLevel.Alert);

                break;
            case "NoIconHUDMessage":
                HudLogger.NoIconHUDMessage(message.Content, message.TimeLeft);

                break;
        }
    }
}
