using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.UI;

internal class TotalOnlineTimeBox : Box
{
    protected override Color textColor => this.config.TotalOnlineTime.TextColor;
    protected override string text => Utility.getHoursMinutesStringFromMilliseconds(this.cabin.owner.millisecondsPlayed);
    protected override Point offset => new(this.config.TotalOnlineTime.XOffset, this.config.TotalOnlineTime.YOffset);

    public TotalOnlineTimeBox(Building building, Cabin cabin, ModConfig config)
        : base(building, cabin, config) { }
}
