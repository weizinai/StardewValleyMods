using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.UI;

internal class CabinOwnerNameBox : Box
{
    protected override Color textColor
    {
        get
        {
            if (Game1.player.Equals(this.cabin.owner))
            {
                return this.config.OwnerColor;
            }

            return Game1.player.team.playerIsOnline(this.cabin.owner.UniqueMultiplayerID) ? this.config.OnlineFarmerColor : this.config.OfflineFarmerColor;
        }
    }

    protected override string text => this.cabin.owner.Name;
    protected override Point offset => new(this.config.NameTagXOffset, this.config.NameTagYOffset);

    public CabinOwnerNameBox(Building building, Cabin cabin, ModConfig config)
        : base(building, cabin, config) { }
}
