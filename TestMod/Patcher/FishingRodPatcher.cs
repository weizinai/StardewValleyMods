using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Tools;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.TestMod.Patcher;

public class FishingRodPatcher : BasePatcher
{
    public FishingRodPatcher()
    {
        FishingRod.baseChanceForTreasure = 1;
    }
    
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction)),
            transpiler: this.GetHarmonyMethod(nameof(StartMinigameEndFunctionTranspiler))
        );
    }

    private static IEnumerable<CodeInstruction> StartMinigameEndFunctionTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R8 && code.operand.Equals(0.25));
        codes[index].operand = 1.0;
        return codes.AsEnumerable();
    }
}