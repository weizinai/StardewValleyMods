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

    public static bool TrySpectateFarmer(string farmerName)
    {
        var farmer = Game1.otherFarmers.FirstOrDefault(x => x.Value.displayName == farmerName).Value;

        if (farmer is null) return false;

        Game1.activeClickableMenu = new SpectatorMenu(farmer.currentLocation, farmer, true);
        return true;
    }
}