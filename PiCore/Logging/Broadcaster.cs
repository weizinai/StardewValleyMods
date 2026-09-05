using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>
///     多人消息发送侧：按模组泛型闭包隔离，经该模组自己的 helper 发送消息。
///     泛型参数 T 为各模组的 <c>ModEntry</c> 标记类型。
/// </summary>
/// <typeparam name="T">模组的 <c>ModEntry</c> 类型。</typeparam>
public static class Broadcaster<T>
{
    /// <summary>
    ///     本闭包内该模组自己的唯一ID。
    /// </summary>
    private static string uniqueId = "";

    /// <summary>
    ///     本闭包内该模组自己的 helper。
    /// </summary>
    private static IModHelper helper = null!;

    /// <summary>
    ///     初始化发送侧：记录模组自己的 helper 与唯一ID，供发送消息时标记来源。
    /// </summary>
    /// <param name="mod">模组入口实例。</param>
    public static void Init(Mod mod)
    {
        helper = mod.Helper;
        uniqueId = mod.ModManifest.UniqueID;
    }

    /// <summary>
    ///     发送 Info 级别提示消息。
    /// </summary>
    /// <param name="message">提示内容。</param>
    /// <param name="playerIDs">目标玩家；null 表示所有玩家。</param>
    public static void Info(string message, long[]? playerIDs = null)
    {
        helper.Multiplayer.SendMessage(new MessageData(message), "Info", new[] { uniqueId }, playerIDs);
    }

    /// <summary>
    ///     发送 Alert 级别提示消息。
    /// </summary>
    /// <param name="message">提示内容。</param>
    /// <param name="playerIDs">目标玩家；null 表示所有玩家。</param>
    public static void Alert(string message, long[]? playerIDs = null)
    {
        helper.Multiplayer.SendMessage(new MessageData(message), "Alert", new[] { uniqueId }, playerIDs);
    }

    /// <summary>
    ///     发送无图标 HUD 提示消息。
    /// </summary>
    /// <param name="message">提示内容。</param>
    /// <param name="timeLeft">显示时长（毫秒）。</param>
    /// <param name="playerIDs">目标玩家；null 表示所有玩家。</param>
    public static void NoIconHUDMessage(string message, float timeLeft = 3500f, long[]? playerIDs = null)
    {
        helper.Multiplayer.SendMessage(new MessageData(message, timeLeft), "NoIconHUDMessage", new[] { uniqueId }, playerIDs);
    }
}
