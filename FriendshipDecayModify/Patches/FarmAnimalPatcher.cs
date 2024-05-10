using System.Reflection.Emit;
using Common.Patch;
using FriendshipDecayModify.Framework;
using HarmonyLib;
using StardewValley;

namespace FriendshipDecayModify.Patches;

public class FarmAnimalPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static int friendshipTowardFarmer;

    public FarmAnimalPatcher(ModConfig config)
    {
        FarmAnimalPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate)),
            GetHarmonyMethod(nameof(DayUpdatePrefix)),
            transpiler: GetHarmonyMethod(nameof(DayUpdateTranspiler))
        );
    }

    private static bool DayUpdatePrefix(FarmAnimal __instance)
    {
        friendshipTowardFarmer = __instance.friendshipTowardFarmer.Value;
        return true;
    }

    private static IEnumerable<CodeInstruction> DayUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        // 抚摸动物友谊修改
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)10));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmAnimalPatcher), nameof(GetPetAnimalModifyForFriendship)));
        // 抚摸动物心情修改
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)50));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmAnimalPatcher), nameof(GetPetAnimalModifyForHappiness)));
        // 喂食动物心情修改
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)100));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmAnimalPatcher), nameof(GetFeedAnimalModifyForHappiness)));
        // 喂食动物友谊修改
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)20));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmAnimalPatcher), nameof(GetFeedAnimalModifyForFriendship)));

        return codes.AsEnumerable();
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