using System.Diagnostics.CodeAnalysis;
using StardewValley.Locations;

namespace TestMod.Framework;

public class VolcanoDungeonPatch
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static bool GenerateLevelPrefix(VolcanoDungeon __instance)
    {
        if (__instance.level.Value is 0 or 5 or 9) return true;
        __instance.layoutIndex.Value = ModEntry.Config.LayoutIndex;
        return true;
    }
}