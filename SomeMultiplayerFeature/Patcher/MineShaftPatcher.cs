using HarmonyLib;
using StardewValley;
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

    private static void OnLeftMinesPostfix()
    {
        if (Game1.IsMultiplayer) MineShaft.activeMines.RemoveAll(mine => mine.mineLevel <= 120 && !mine.farmers.Any());
    }
}