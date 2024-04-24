using Common.Patch;
using HarmonyLib;
using StardewValley.Locations;
using TestMod.Framework;

namespace TestMod.Patches;

public class VolcanoDungeonPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public VolcanoDungeonPatcher(ModConfig config)
    {
        VolcanoDungeonPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(RequireMethod<VolcanoDungeon>(nameof(VolcanoDungeon.GenerateLevel)), GetHarmonyMethod(nameof(GenerateLevelPrefix)));
    }

    private static bool GenerateLevelPrefix(VolcanoDungeon __instance)
    {
        if (__instance.level.Value is 0 or 5 or 9) return true;
        __instance.layoutIndex.Value = config.VolcanoDungeonMap;
        return true;
    }
}