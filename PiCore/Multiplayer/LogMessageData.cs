namespace weizinai.StardewValleyMod.PiCore.Multiplayer;

/// <summary>
/// 控制台日志消息的网络载荷，由 <see cref="Logging.LogReceiver" /> 读出并输出。属性名是线协议的一部分，改动会让新旧版本的 PiCore 互相读不懂，因此保持原值。
/// </summary>
public class LogMessageData
{
    public string Content { get; set; }

    public LogMessageData(string content)
    {
        this.Content = content;
    }
}
