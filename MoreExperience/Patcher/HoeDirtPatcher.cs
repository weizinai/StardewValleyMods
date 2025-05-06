using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MoreExperience.Patcher;

internal class HoeDirtPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<HoeDirt>(nameof(HoeDirt.performToolAction)),
            prefix: this.GetHarmonyMethod(nameof(PerformToolActionPrefix))
        );
    }

    // 添加浇水获得5点耕种经验
    private static void PerformToolActionPrefix(Tool t, HoeDirt __instance)
    {
        if (t is WateringCan && __instance.state.Value == HoeDirt.dry && __instance.crop != null)
        {
            t.getLastFarmerToUse().gainExperience(Farmer.farmingSkill, 5);
        }
    }
}