using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.UI;

internal class CabinOwnerNameBox : Box
{
    protected override Color TextColor
    {
        get
        {
            if (Game1.player.Equals(this.Cabin.owner))
            {
                return this.Config.OwnerColor;
            }

            return Game1.player.team.playerIsOnline(this.Cabin.owner.UniqueMultiplayerID) ? this.Config.OnlineFarmerColor : this.Config.OfflineFarmerColor;
        }
    }

    protected override string Text => this.Cabin.owner.displayName;
    protected override Point Offset => new(this.Config.NameTagXOffset, this.Config.NameTagYOffset);

    public CabinOwnerNameBox(Building building, Cabin cabin, ModConfig config)
        : base(building, cabin, config) { }
}