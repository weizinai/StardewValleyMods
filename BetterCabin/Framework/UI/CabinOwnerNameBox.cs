using BetterCabin.Framework.Config;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BetterCabin.Framework.UI;

internal class CabinOwnerNameBox : Box
{
    protected override Color TextColor
    {
        get
        {
            if (Game1.player.Equals(Cabin.owner))
            {
                return Config.OwnerColor;
            }

            return Game1.player.team.playerIsOnline(Cabin.owner.UniqueMultiplayerID) ? Config.OnlineFarmerColor : Config.OfflineFarmerColor;
        }
    }
    protected override string Text => Cabin.owner.displayName;
    protected override Point Offset => new(Config.NameTagXOffset, Config.NameTagYOffset);

    public CabinOwnerNameBox(Building building, Cabin cabin, ModConfig config) 
        : base(building, cabin, config)
    {
    }
}