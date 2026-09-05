using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;

internal class NPCPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public NPCPatcher(ModConfig config)
    {
        NPCPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<NPC>(nameof(NPC.receiveGift)),
            transpiler: this.GetHarmonyMethod(nameof(ReceiveGiftTranspiler))
        );
    }

    // 礼物修改
    private static IEnumerable<CodeInstruction> ReceiveGiftTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldc_R4, -40f))
            .SetInstruction(CodeInstruction.Call(typeof(NPCPatcher), nameof(GetHateGiftModify)))
            .MatchEndForward(new CodeMatch(OpCodes.Ldc_R4, -20f))
            .SetInstruction(CodeInstruction.Call(typeof(NPCPatcher), nameof(GetDislikeGiftModify)));

        return codeMatcher.Instructions();
    }

    private static float GetHateGiftModify()
    {
        return -config.HateGiftModify;
    }

    private static float GetDislikeGiftModify()
    {
        return -config.DislikeGiftModify;
    }
}