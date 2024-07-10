using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class FarmerPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Farmer>(nameof(Farmer.gainExperience)),
            prefix: this.GetHarmonyMethod(nameof(GainExperiencePrefix))
        );
    }

    // 精通后钓鱼经验翻倍
    private static void GainExperiencePrefix(int which, ref int howMuch)
    {
        if (Game1.player.locationsVisited.Contains("MasteryCave") && which == Farmer.fishingSkill) howMuch *= 2;
    }
}