using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace weizinai.StardewValleyMod.PiCore.Multiplayer;

/// <summary>
/// 多人消息发送侧：把日志与 HUD 提示发到其他玩家的机器，经该模组自己的 helper 发送（<c>T</c> 传调用方自己的 <c>ModEntry</c>，静态状态按它隔离）。
/// 广播不回显给发送者——给自己看请走 <see cref="Logging.Logger{T}" /> 或 <see cref="Hud.HudLogger" />。
/// 三个方法的 <c>playerIDs</c> 传 <c>null</c> 表示除本机玩家外的所有在线玩家。
/// </summary>
public static class Broadcaster<T>
{
    // 收件人白名单必须是 PiCore 自己：订阅 ModMessageReceived 的是 PiCore，而 SMAPI 只把消息投给白名单里出现的那些模组的处理器
    // 必须与 PiCore/manifest.json 的 UniqueID 一致，写错会让所有广播静默落空
    private const string ReceiverModId = "weizinai.PiCore";

    private static IModHelper helper = null!;

    public static void Init(Mod mod)
    {
        helper = mod.Helper;
    }

    public static void Info(string message, long[]? playerIDs = null)
    {
        Send(new LogMessageData(message), MessageTypes.Info, playerIDs);
    }

    public static void Alert(string message, long[]? playerIDs = null)
    {
        Send(new LogMessageData(message), MessageTypes.Alert, playerIDs);
    }

    /// <summary>向其他玩家发送无图标 HUD 提示；<c>timeLeft</c> 单位为毫秒。</summary>
    public static void NoIconHUDMessage(string message, float timeLeft = 3500f, long[]? playerIDs = null)
    {
        Send(new HudMessageData(message, timeLeft), MessageTypes.NoIconHudMessage, playerIDs);
    }

    private static void Send<TData>(TData messageData, string messageType, long[]? playerIDs)
    {
        helper.Multiplayer.SendMessage(messageData, messageType, new[] { ReceiverModId }, ResolvePlayerIds(playerIDs));
    }

    // 广播是「告诉别人」：null 解析为本机玩家之外的所有在线玩家
    // 无人在线时解析为空数组，SMAPI 只写一条 verbose 日志后返回——不抛异常，也没有用户可见噪音
    private static long[] ResolvePlayerIds(long[]? playerIDs)
    {
        if (playerIDs is not null)
        {
            return playerIDs;
        }

        var localPlayerId = Game1.player.UniqueMultiplayerID;

        return Game1.getOnlineFarmers()
            .Where(farmer => farmer.UniqueMultiplayerID != localPlayerId)
            .Select(farmer => farmer.UniqueMultiplayerID)
            .ToArray();
    }
}
