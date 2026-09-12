using StardewValley;

namespace weizinai.StardewValleyMod.PiCore.Hud;

// cref 必须带 PiCore. 前缀：本文件 using 了 StardewValley，简写的 Multiplayer 会被同名的 StardewValley.Multiplayer 类带偏，IDE 解析不到。
/// <summary>本机 HUD 提示通道：只显示屏幕提示，与 monitor 及日志初始化无关；要让其他玩家看到提示请走 <see cref="PiCore.Multiplayer.Broadcaster{T}" />。</summary>
public static class HudLogger
{
    /// <summary>无图标 HUD 提示；<c>timeLeft</c> 单位为毫秒。</summary>
    public static void NoIconHUDMessage(string message, float timeLeft = 3500f)
    {
        Game1.addHUDMessage(new HUDMessage(message, timeLeft) { noIcon = true });
    }

    /// <summary>错误图标 HUD 提示；<c>timeLeft</c> 单位为毫秒。</summary>
    public static void ErrorHUDMessage(string message, float timeLeft = 3500f)
    {
        Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = timeLeft });
    }
}
