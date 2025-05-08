using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;

internal class FarmerPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public FarmerPatcher(ModConfig config)
    {
        FarmerPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Farmer>(nameof(Farmer.resetFriendshipsForNewDay)),
            transpiler: this.GetHarmonyMethod(nameof(ResetFriendshipsForNewDayTranspiler))
        );
    }

    // 每日对话修改
    private static IEnumerable<CodeInstruction> ResetFriendshipsForNewDayTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            // 配偶
            .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)-20))
            .SetInstruction(CodeInstruction.Call(typeof(FarmerPatcher), nameof(GetDailyGreetingModifyForSpouse)))
            // 对象
            .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)-8))
            .SetInstruction(CodeInstruction.Call(typeof(FarmerPatcher), nameof(GetDailyGreetingModifyForDatingVillager)))
            // 村民
            .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)-2))
            .SetInstruction(CodeInstruction.Call(typeof(FarmerPatcher), nameof(GetDailyGreetingModifyForVillager)));

        return codeMatcher.Instructions();
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