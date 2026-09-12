using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Minigames;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.TestMod.Config;

namespace weizinai.StardewValleyMod.TestMod.Patcher;

internal class CalicoJackPatcher : BasePatcher
{
    /// <inheritdoc />
    public override void Apply(Harmony harmony)
    {
        this.Patch<CalicoJack>(harmony, nameof(CalicoJack.tick), PatchKind.Transpiler, nameof(TickTranspiler));
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
