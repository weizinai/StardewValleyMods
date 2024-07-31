using StardewValley;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal class PlayerToKickData
{
    public Farmer Player { get; set; }
    public int TimeLeft { get; set; } = 10;

    public PlayerToKickData(Farmer player)
    {
        this.Player = player;
    }
}