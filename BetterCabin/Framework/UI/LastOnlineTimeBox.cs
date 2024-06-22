using BetterCabin.Framework.Config;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BetterCabin.Framework.UI;

internal class LastOnlineTimeBox : Box
{
    protected override Color TextColor => config.LastOnlineTime.TextColor;
    protected override string Text => Utility.getDateString((int)(Game1.stats.DaysPlayed - cabin.owner.disconnectDay.Value));
    protected override Point Offset => new(config.LastOnlineTime.XOffset, config.LastOnlineTime.YOffset);

    public LastOnlineTimeBox(Building building, Cabin cabin, ModConfig config) : base(building, cabin, config)
    {
    }
}