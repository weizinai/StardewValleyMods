namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>
/// 多人消息载体：经 SMAPI 网络序列化传输的消息内容。
/// </summary>
public class MessageData
{
    /// <summary>
    /// 消息内容。
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// HUD 提示显示时长（毫秒）。
    /// </summary>
    public float TimeLeft { get; set; }

    /// <summary>
    /// 构造消息数据。
    /// </summary>
    /// <param name="content">消息内容。</param>
    /// <param name="timeLeft">HUD 提示显示时长（毫秒）。</param>
    public MessageData(string content, float timeLeft = 3500f)
    {
        this.Content = content;
        this.TimeLeft = timeLeft;
    }
}
