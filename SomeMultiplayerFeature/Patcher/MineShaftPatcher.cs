using HarmonyLib;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class MineShaftPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<MineShaft>(nameof(MineShaft.OnLeftMines)),
            postfix: this.GetHarmonyMethod(nameof(OnLeftMinesPostfix))
        );
    }

    // 矿井即使刷新
    private static void OnLeftMinesPostfix()
    {
        MineShaft.activeMines.RemoveAll(mine => mine.mineLevel <= 120 && !mine.farmers.Any());
    }
}