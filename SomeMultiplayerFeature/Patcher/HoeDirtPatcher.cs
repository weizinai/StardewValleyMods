using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class HoeDirtPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<HoeDirt>(nameof(HoeDirt.performToolAction)),
            prefix: this.GetHarmonyMethod(nameof(PerformToolActionPrefix))
        );
    }

    // 浇水获得4点耕种经验
    private static void PerformToolActionPrefix(Tool t, HoeDirt __instance)
    {
        if (t is WateringCan && __instance.state.Value == HoeDirt.dry && __instance.crop != null)
        {
            t.getLastFarmerToUse().gainExperience(Farmer.farmingSkill, 4);
        }
    }
}