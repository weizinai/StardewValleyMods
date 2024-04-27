using System.Reflection;
using System.Reflection.Emit;
using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using StardewValley;
using StardewValley.Quests;

namespace HelpWanted.Patches;

public class ItemDeliveryQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public ItemDeliveryQuestPatcher(ModConfig config)
    {
        ItemDeliveryQuestPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.loadQuestInfo)),
            transpiler: GetHarmonyMethod(nameof(LoadQuestInfoTranspiler))
        );
        harmony.Patch(
            RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.GetGoldRewardPerItem)),
            postfix: GetHarmonyMethod(nameof(GetGoldRewardPerItemPostfix))
        );
        harmony.Patch(
            RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.checkIfComplete)),
            transpiler: GetHarmonyMethod(nameof(CheckIfCompleteTranspiler))
        );
    }
    
    private static IEnumerable<CodeInstruction> CheckIfCompleteTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4 && (int)code.operand == 150);
        codes[index].operand = config.ItemDeliveryFriendshipGain;
        return codes.AsEnumerable();
    }
    
    private static void GetGoldRewardPerItemPostfix(ref int __result)
    {
        __result = (int)(__result * config.ItemDeliveryRewardMultiplier);
    }
    
    private static IEnumerable<CodeInstruction> LoadQuestInfoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        var start = false;
        var found1 = false;
        var found2 = false;
        for (var i = 0; i < codes.Count; i++)
        {
            switch (start)
            {
                case true when !found1 && codes[i].opcode == OpCodes.Ldc_R8:
                    codes[i].operand = -0.1;
                    found1 = true;
                    break;
                case false when codes[i].opcode == OpCodes.Ldstr && (string)codes[i].operand == "Cooking":
                    start = true;
                    break;
                default:
                {
                    if (!found2 && codes[i].opcode == OpCodes.Call && (MethodInfo)codes[i].operand ==
                        AccessTools.Method(typeof(Utility), nameof(Utility.possibleCropsAtThisTime)))
                    {
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetPossibleCrops))));
                        i++;
                        found2 = true;
                    }

                    break;
                }
            }

            if (found1 && found2)
                break;
        }

        return codes.AsEnumerable();
    }
}