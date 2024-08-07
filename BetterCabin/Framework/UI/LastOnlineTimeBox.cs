using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.UI;

internal class LastOnlineTimeBox : Box
{
    protected override Color TextColor => this.Config.LastOnlineTime.TextColor;
    protected override string Text => Utility.getDateString(-((int)Game1.stats.DaysPlayed - this.Cabin.owner.disconnectDay.Value));
    protected override Point Offset => new(this.Config.LastOnlineTime.XOffset, this.Config.LastOnlineTime.YOffset);

    public LastOnlineTimeBox(Building building, Cabin cabin, ModConfig config)
        : base(building, cabin, config) { }
}