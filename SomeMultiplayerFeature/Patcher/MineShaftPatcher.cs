using HarmonyLib;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

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

    // 矿井即时刷新
    private static void OnLeftMinesPostfix()
    {
        MineshaftHandler.RefreshMineshaft();
        Log.NoIconHUDMessage("矿井已刷新", 500f);
    }
}