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
        // harmony.Patch(
        //     original: this.RequireMethod<MineShaft>(nameof(MineShaft.tryToAddOreClumps)),
        //     transpiler: this.GetHarmonyMethod(nameof(TryToAddOreClumpsTranspiler))
        // );
    }

    // 矿井即时刷新
    private static void OnLeftMinesPostfix()
    {
        MineshaftHandler.RefreshMineshaft();
        Log.NoIconHUDMessage("矿井已刷新", 500f);
    }

    // private static IEnumerable<CodeInstruction> TryToAddOreClumpsTranspiler(IEnumerable<CodeInstruction> instructions)
    // {
    //     var codes = instructions.ToList();
    //
    //     var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R8 && code.operand.Equals(0.55));
    //     codes[index].operand = 2.0;
    //     index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_R8 && code.operand.Equals(0.25));
    //     codes[index].operand = 1.0;
    //
    //     return codes.AsEnumerable();
    // }
}