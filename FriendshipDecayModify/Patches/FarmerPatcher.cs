using System.Reflection.Emit;
using Common.Patch;
using FriendshipDecayModify.Framework;
using HarmonyLib;
using StardewValley;
using StardewValley.Characters;

namespace FriendshipDecayModify.Patches;

public class FarmerPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public FarmerPatcher(ModConfig config)
    {
        FarmerPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<Farmer>(nameof(Farmer.resetFriendshipsForNewDay)),
            transpiler: GetHarmonyMethod(nameof(ResetFriendshipsForNewDayTranspiler))
        );
    }

    // 每日对话修改
    private static IEnumerable<CodeInstruction> ResetFriendshipsForNewDayTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == -20);
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmerPatcher), nameof(GetDailyGreetingModifyForSpouse)));
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == -8);
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmerPatcher), nameof(GetDailyGreetingModifyForDatingVillager)));
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == -2);
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmerPatcher), nameof(GetDailyGreetingModifyForVillager)));

        return codes.AsEnumerable();
    }

    private static int GetDailyGreetingModifyForVillager()
    {
        return -config.DailyGreetingModifyForVillager;
    }

    private static int GetDailyGreetingModifyForDatingVillager()
    {
        return -config.DailyGreetingModifyForDatingVillager;
    }

    private static int GetDailyGreetingModifyForSpouse()
    {
        return -config.DailyGreetingModifyForSpouse;
    }
}