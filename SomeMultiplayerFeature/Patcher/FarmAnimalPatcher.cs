using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class FarmAnimalPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.pet)),
            transpiler: this.GetHarmonyMethod(nameof(PetTranspiler))
        );
    }

    // 修改抚摸动物获得的经验为50点
    private static IEnumerable<CodeInstruction> PetTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience))));
        codes[index - 1] = new CodeInstruction(OpCodes.Ldc_I4, 50);
        Log.Info("修改抚摸动物获得的经验为50点");

        return codes.AsEnumerable();
    }
}