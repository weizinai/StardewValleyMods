namespace weizinai.StardewValleyMod.PiCore.Multiplayer;

/// <summary>
/// HUD 提示消息的网络载荷，由 <see cref="Hud.HudReceiver" /> 读出并显示。属性名是线协议的一部分，改动会让新旧版本的 PiCore 互相读不懂，因此保持原值。
/// </summary>
public class HudMessageData
{
    public string Content { get; set; }

    public float TimeLeft { get; set; }

    /// <summary>构造 HUD 提示消息载荷；<c>timeLeft</c> 单位为毫秒。</summary>
    public HudMessageData(string content, float timeLeft = 3500f)
    {
        this.Content = content;
        this.TimeLeft = timeLeft;
    }
}
