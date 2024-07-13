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

    public static bool IsCabinWithOwner(this Building building, out Cabin? cabin)
    {
        if (building.IsCabin(out var value))
        {
            cabin = value;
            return !value!.owner.isUnclaimedFarmhand;
        }

        cabin = null;
        return false;
    }
}