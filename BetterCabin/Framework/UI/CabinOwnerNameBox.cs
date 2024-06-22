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
            if (Game1.player.Equals(cabin.owner))
            {
                return config.OwnerColor;
            }

            return Game1.player.team.playerIsOnline(cabin.owner.UniqueMultiplayerID) ? config.OnlineFarmerColor : config.OfflineFarmerColor;
        }
    }
    protected override string Text => cabin.owner.displayName;
    protected override Point Offset => new(config.NameTagXOffset, config.NameTagYOffset);

    public CabinOwnerNameBox(Building building, Cabin cabin, ModConfig config) 
        : base(building, cabin, config)
    {
    }
}