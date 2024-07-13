namespace weizinai.StardewValleyMod.Common.Log;

internal class ModMessage
{
    public string Content { get; set; }
    public float TimeLeft { get; set; }

    public ModMessage(string content, float timeLeft = 3500f)
    {
        this.Content = content;
        this.TimeLeft = timeLeft;
    }
}