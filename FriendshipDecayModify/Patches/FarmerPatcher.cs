using Common.Patch;
using FriendshipDecayModify.Framework;
using HarmonyLib;
using StardewValley;

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

        var index = codes.FindIndex(code => code.operand.Equals(-20));
        codes[index].operand = config.DailyGreetingModifyForSpouse;
        index = codes.FindIndex(code => code.operand.Equals(-8));
        codes[index].operand = config.DailyGreetingModifyForDatingVillager;
        index = codes.FindIndex(code => code.operand.Equals(-2));
        codes[index].operand = config.DailyGreetingModifyForVillager;

        return codes.AsEnumerable();
    }
}