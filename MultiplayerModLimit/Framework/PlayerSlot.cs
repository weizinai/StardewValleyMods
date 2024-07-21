namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class PlayerSlot
{
    public long Id { get; }
    public int TimeLeft { get; set; }

    public PlayerSlot(long id, int timeLeft)
    {
        this.Id = id;
        this.TimeLeft = timeLeft;
    }
}