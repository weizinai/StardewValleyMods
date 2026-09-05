namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal class PlayerToKickData
{
    public long Id { get; }
    public int TimeLeft { get; set; }

    public PlayerToKickData(long id, int timeLeft)
    {
        this.Id = id;
        this.TimeLeft = timeLeft;
    }
}