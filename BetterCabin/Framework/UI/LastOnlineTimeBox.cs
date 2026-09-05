using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.UI;

internal class LastOnlineTimeBox : Box
{
    protected override Color textColor => this.config.LastOnlineTime.TextColor;
    protected override string text => Utility.getDateString(-((int)Game1.stats.DaysPlayed - this.cabin.owner.disconnectDay.Value));
    protected override Point offset => new(this.config.LastOnlineTime.XOffset, this.config.LastOnlineTime.YOffset);

    public LastOnlineTimeBox(Building building, Cabin cabin, ModConfig config)
        : base(building, cabin, config) { }
}
