using StardewValley.Buildings;
using StardewValley.Locations;

namespace weizinai.StardewValleyMod.Common.Extension;

internal static class BuildingExtension
{
    public static bool IsCabin(this Building building)
    {
        return building.GetIndoors() is Cabin;
    }
}