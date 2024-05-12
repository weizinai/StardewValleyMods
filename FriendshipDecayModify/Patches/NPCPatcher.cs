using System.Reflection;
using System.Reflection.Emit;
using Common.Patch;
using FriendshipDecayModify.Framework;
using HarmonyLib;
using StardewValley;

namespace FriendshipDecayModify.Patches;

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
            RequireMethod<NPC>(nameof(NPC.receiveGift)),
            transpiler: GetHarmonyMethod(nameof(ReceiveGiftTranspiler))
        );
    }

    // 礼物修改
    private static IEnumerable<CodeInstruction> ReceiveGiftTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_R4 && code.operand.Equals(-40f));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetHateGiftModify)));
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_R4 && code.operand.Equals(-20f));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetDislikeGiftModify)));

        return codes.AsEnumerable();
    }

    private static float GetHateGiftModify()
    {
        return -config.HateGiftModify;
    }

    private static float GetDislikeGiftModify()
    {
        return -config.DislikeGiftModify;
    }

    private static MethodInfo GetMethod(string name)
    {
        return AccessTools.Method(typeof(NPCPatcher), name) ??
               throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(FarmerPatcher), name)}.");
    }
}