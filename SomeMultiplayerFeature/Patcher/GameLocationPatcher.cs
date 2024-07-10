using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class GameLocationPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<GameLocation>("breakStone"),
            postfix: this.GetHarmonyMethod(nameof(BreakStonePostfix))
        );
    }

    // 修改铜矿获得的经验为14点
    // 修改铁矿获得的经验为16点
    private static void BreakStonePostfix(string stoneId, Farmer who)
    {
        var miningSkill = Farmer.miningSkill;

        switch (stoneId)
        {
            case "751":
                who.gainExperience(miningSkill, 9);
                break;
            case "850":
                who.gainExperience(miningSkill, 4);
                break;
        }
    }
}