using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Minigames;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.TestMod.Framework;

namespace weizinai.StardewValleyMod.TestMod.Patcher;

internal class CalicoJackPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<CalicoJack>(nameof(CalicoJack.tick)),
            transpiler: this.GetHarmonyMethod(nameof(TickTranspiler))
        );
    }

    private static IEnumerable<CodeInstruction> TickTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R8 && code.operand.Equals(0.0005));
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CalicoJackPatcher), nameof(GetCardChance))));

        return codes.AsEnumerable();
    }

    private static double GetCardChance(double originChance)
    {
        var config = ModConfig.Instance.CardChance;
        return config.IsEnabled ? config.Value : originChance;
    }
}