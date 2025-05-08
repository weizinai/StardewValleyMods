using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Minigames;
using weizinai.StardewValleyMod.PiCore.Patcher;
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
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            .MatchStartForward(new CodeMatch(OpCodes.Ldc_R8, 0.0005))
            .Advance(1)
            .Insert(CodeInstruction.Call(typeof(CalicoJackPatcher), nameof(GetCardChance)));

        return codeMatcher.Instructions();
    }

    private static double GetCardChance(double originChance)
    {
        var config = ModConfig.Instance.CardChance;
        return config.IsEnabled ? config.Value : originChance;
    }
}