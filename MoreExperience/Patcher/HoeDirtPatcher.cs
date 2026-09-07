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
        this.Patch<HoeDirt>(harmony, nameof(HoeDirt.performToolAction), PatchKind.Prefix, nameof(PerformToolActionPrefix));
    }

    // 添加浇水获得5点耕种经验
    private static void PerformToolActionPrefix(Tool t, HoeDirt __instance)
    {
        if (t is WateringCan && __instance.state.Value == HoeDirt.dry && __instance.crop != null) t.getLastFarmerToUse().gainExperience(Farmer.farmingSkill, 5);
    }
}
