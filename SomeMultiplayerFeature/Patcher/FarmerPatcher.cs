using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class FarmerPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<Farmer>(harmony, nameof(Farmer.Update), PatchKind.Transpiler, nameof(UpdateTranspiler));

        Logger<ModEntry>.Info("修改体力再生速度为原来的5倍");
    }

    private static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindLastIndex(code => code.opcode == OpCodes.Ldc_I4 && code.operand.Equals(500));
        codes[index].operand = 100;

        return codes.AsEnumerable();
    }
}
