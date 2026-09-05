namespace weizinai.StardewValleyMod.Common;

internal class MessageData
{
    public string Content { get; set; }
    public float TimeLeft { get; set; }

    public MessageData(string content, float timeLeft = 3500f)
    {
        this.Content = content;
        this.TimeLeft = timeLeft;
    }
}