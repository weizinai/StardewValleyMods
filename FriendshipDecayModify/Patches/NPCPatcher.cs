using System.Reflection.Emit;
using Common.Patch;
using FriendshipDecayModify.Framework;
using HarmonyLib;
using StardewValley;

namespace FriendshipDecayModify.Patches;

public class NPCPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public NPCPatcher(ModConfig config)
    {
        NPCPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<NPC>(nameof(NPC.receiveGift)),
            transpiler: GetHarmonyMethod(nameof(ReceiveGiftTranspiler))
        );
    }

    private static IEnumerable<CodeInstruction> ReceiveGiftTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R4 && code.operand.Equals(-40f));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NPCPatcher), nameof(GetHateGiftModify)));
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_R4 && code.operand.Equals(-20f));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NPCPatcher), nameof(GetDislikeGiftModify)));

        return codes.AsEnumerable();
    }

    private static float GetHateGiftModify()
    {
        return -(float)config.HateGiftModify;
    }
    
    private static float GetDislikeGiftModify()
    {
        return -(float)config.DislikeGiftModify;
    }
}