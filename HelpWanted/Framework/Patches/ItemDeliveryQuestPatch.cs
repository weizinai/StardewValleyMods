using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

namespace HelpWanted.Framework.Patches;

public class ItemDeliveryQuestPatch
{
    public static IEnumerable<CodeInstruction> LoadQuestInfoTranspiler(IEnumerable<CodeInstruction> instructions)
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
                    // codes[i].operand = -0.1;
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
    
    public static void GetGoldRewardPerItemPostfix(ref int __result)
    {
        var config = ModEntry.Config;
        __result = (int)(__result * config.ItemDeliveryRewardModifier);
    }
}