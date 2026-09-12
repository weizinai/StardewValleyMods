namespace weizinai.StardewValleyMod.PiCore.Multiplayer;

/// <summary>
/// 多人消息类型名：发送侧与接收侧共用同一份定义，避免各自漂移出不同的字符串。这些字符串会进入网络线协议，改动会让新旧版本的 PiCore 互相读不懂，因此保持原值。
/// </summary>
internal static class MessageTypes
{
    public const string Info = "Info";

    public const string Alert = "Alert";

    public const string NoIconHudMessage = "NoIconHUDMessage";
}
