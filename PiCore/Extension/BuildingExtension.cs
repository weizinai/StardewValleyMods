using System.Diagnostics.CodeAnalysis;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace weizinai.StardewValleyMod.PiCore.Extension;

public static class BuildingExtension
{
    public static bool IsCabin(this Building building, [NotNullWhen(true)] out Cabin? cabin)
    {
        if (building.GetIndoors() is Cabin tempCabin)
        {
            cabin = tempCabin;
            return true;
        }

        cabin = null;
        return false;
    }

    public static bool IsCabinWithOwner(this Building building, [NotNullWhen(true)] out Cabin? cabin)
    {
        if (building.IsCabin(out var value))
        {
            cabin = value;
            return !value.owner.isUnclaimedFarmhand;
        }

        cabin = null;
        return false;
    }
}