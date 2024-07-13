using StardewValley.Buildings;
using StardewValley.Locations;

namespace weizinai.StardewValleyMod.Common.Extension;

internal static class BuildingExtension
{
    public static bool IsCabin(this Building building, out Cabin? cabin)
    {
        if (building.GetIndoors() is Cabin tempCabin)
        {
            cabin = tempCabin;
            return true;
        }
        cabin = null;
        return false;
    }
}