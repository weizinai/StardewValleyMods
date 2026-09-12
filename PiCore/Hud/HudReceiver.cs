using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Multiplayer;

namespace weizinai.StardewValleyMod.PiCore.Hud;

/// <summary>
/// 多人消息接收侧：直接把对端发来的 HUD 提示显示出来。不查 monitor 注册表——HUD 显示与日志初始化无关，只发 HUD 的模组无需先初始化日志。
/// </summary>
internal static class HudReceiver
{
    /// <summary>其余消息类型留给别的接收侧，这里只处理无图标 HUD 提示。</summary>
    internal static void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.Type != MessageTypes.NoIconHudMessage)
        {
            return;
        }

        var message = e.ReadAs<HudMessageData>();

        HudLogger.NoIconHUDMessage(message.Content, message.TimeLeft);
    }
}
