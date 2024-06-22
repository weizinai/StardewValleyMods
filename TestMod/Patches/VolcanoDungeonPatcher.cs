using weizinai.StardewValleyMod.Common.Patcher;
using HarmonyLib;
using StardewValley.Locations;
using weizinai.StardewValleyMod.TestMod.Framework;

namespace weizinai.StardewValleyMod.TestMod.Patches;

internal class VolcanoDungeonPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public VolcanoDungeonPatcher(ModConfig config)
    {
        VolcanoDungeonPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
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