using BetterCabin.Framework.Config;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BetterCabin.Framework.UI;

internal class TotalOnlineTimeBox : Box
{
    protected override Color TextColor => config.TotalOnlineTime.TextColor;
    protected override string Text => Utility.getHoursMinutesStringFromMilliseconds(cabin.owner.millisecondsPlayed);
    protected override Point Offset => new(config.TotalOnlineTime.XOffset, config.TotalOnlineTime.YOffset);

    public TotalOnlineTimeBox(Building building, Cabin cabin, ModConfig config) 
        : base(building, cabin, config)
    {
    }
}