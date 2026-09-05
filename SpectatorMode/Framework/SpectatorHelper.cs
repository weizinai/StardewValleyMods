using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewValley;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal static class SpectatorHelper
{
    public static bool TrySpectateLocation(string locationName)
    {
        var location = Game1.getLocationFromName(locationName);

        if (location is null) return false;

        Game1.activeClickableMenu = new SpectatorMenu(location);
        return true;
    }

    public static bool TrySpectateFarmer(string farmerName, [NotNullWhen(true)] out SpectatorMenu? menu)
    {
        var farmer = Game1.getOnlineFarmers().FirstOrDefault(x => x.displayName == farmerName);

        if (farmer is null)
        {
            menu = null;
            return false;
        }

        menu = new SpectatorMenu(farmer.currentLocation, farmer);
        Game1.activeClickableMenu = menu;
        return true;
    }
}