using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;

internal class FarmAnimalPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static int friendshipTowardFarmer;

    public FarmAnimalPatcher(ModConfig config)
    {
        FarmAnimalPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate)), this.GetHarmonyMethod(nameof(DayUpdatePrefix)),
            transpiler: this.GetHarmonyMethod(nameof(DayUpdateTranspiler))
        );
    }

    private static bool DayUpdatePrefix(FarmAnimal __instance)
    {
        friendshipTowardFarmer = __instance.friendshipTowardFarmer.Value;
        return true;
    }

    private static IEnumerable<CodeInstruction> DayUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            // 抚摸动物友谊修改
            .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)10))
            .SetInstructionAndAdvance(CodeInstruction.Call(typeof(FarmAnimalPatcher), nameof(GetPetAnimalModifyForFriendship)))
            // 抚摸动物心情修改
            .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)50))
            .SetInstructionAndAdvance(CodeInstruction.Call(typeof(FarmAnimalPatcher), nameof(GetPetAnimalModifyForHappiness)))
            // 喂食动物心情修改
            .MatchEndForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)100))
            .SetInstructionAndAdvance(CodeInstruction.Call(typeof(FarmAnimalPatcher), nameof(GetFeedAnimalModifyForHappiness)))
            // 喂食动物友谊修改
            .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)20))
            .SetInstruction(CodeInstruction.Call(typeof(FarmAnimalPatcher), nameof(GetFeedAnimalModifyForFriendship)));

        return codeMatcher.Instructions();
    }

    // 抚摸动物友谊修改
    private static int GetPetAnimalModifyForFriendship()
    {
        var petAnimalDecay = config.PetAnimalModifyForFriendship - friendshipTowardFarmer / 200;
        return petAnimalDecay < 0 ? petAnimalDecay : config.PetAnimalModifyForFriendship;
    }

    // 抚摸动物心情修改
    private static int GetPetAnimalModifyForHappiness()
    {
        return config.PetAnimalModifyForHappiness;
    }

    // 喂食动物友谊修改
    private static int GetFeedAnimalModifyForFriendship()
    {
        return config.FeedAnimalModifyForFriendship;
    }

    // 喂食动物心情修改
    private static int GetFeedAnimalModifyForHappiness()
    {
        return config.FeedAnimalModifyForHappiness;
    }
}