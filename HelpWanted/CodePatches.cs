using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HelpWanted.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

// ReSharper disable InconsistentNaming

namespace HelpWanted;

internal partial class ModEntry
{
    private void HarmonyPatch()
    {
        var harmony = new Harmony("aedenthorn.HelpWanted");
        harmony.Patch(
            AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
            new HarmonyMethod(typeof(Billboard_draw_Patch), "Prefix")
        );
        harmony.Patch(
            AccessTools.Method(typeof(Billboard), "receiveLeftClick"),
            postfix: new HarmonyMethod(typeof(Billboard_receiveLeftClick_Patch), "Postfix")
        );
        harmony.Patch(AccessTools.Method(typeof(Utility), "getRandomItemFromSeason",
                new[] { typeof(Season), typeof(int), typeof(bool), typeof(bool) }),
            prefix: new HarmonyMethod(typeof(Utility_getRandomItemFromSeason_Patch), "Prefix"),
            transpiler: new HarmonyMethod(typeof(Utility_getRandomItemFromSeason_Patch), "Transpiler")
        );
        harmony.Patch(
            AccessTools.Method(typeof(ItemDeliveryQuest), "loadQuestInfo"),
            transpiler: new HarmonyMethod(typeof(ItemDeliveryQuest_loadQuestInfo_Patch), "Transpiler")
        );
    }

    public class Billboard_draw_Patch
    {
        public static bool Prefix(bool ___dailyQuestBoard)
        {
            if (!Config.ModEnabled || !___dailyQuestBoard || Game1.activeClickableMenu.GetType() != typeof(Billboard))
                return true;
            Game1.activeClickableMenu = new OrdersBillboard();
            return false;
        }
    }

    public class Billboard_receiveLeftClick_Patch
    {
        public static void Postfix(Billboard __instance, bool ___dailyQuestBoard, int x, int y)
        {
            if (!Config.ModEnabled || !___dailyQuestBoard || Game1.activeClickableMenu is not OrdersBillboard)
                return;
            __instance.acceptQuestButton.visible = true;
            if (__instance.acceptQuestButton.containsPoint(x, y))
            {
                Game1.questOfTheDay.daysLeft.Value = Config.ModEnabled ? Config.QuestDays : 2;
                Game1.player.acceptedDailyQuest.Set(false);
                Game1.netWorldState.Value.SetQuestOfTheDay(null);
                OrdersBillboard.QuestDataDictionary.Remove(OrdersBillboard.ShowingQuest);
                OrdersBillboard.QuestNotes.RemoveAll(option => option.myID == OrdersBillboard.ShowingQuest);
                OrdersBillboard.QuestBillboard = null;
            }
            else if (__instance.upperRightCloseButton.containsPoint(x, y))
            {
                OrdersBillboard.QuestBillboard = null;
            }
        }
    }

    public class Utility_getRandomItemFromSeason_Patch
    {
        public static void Prefix(ref int randomSeedAddition)
        {
            if (!Config.ModEnabled || !gettingQuestDetails)
                return;
            randomSeedAddition += Random.Next();
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Utility.getRandomItemFromSeason");

            var codes = new List<CodeInstruction>(instructions);
            codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Ldloc_1));
            codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetRandomItem))));

            return codes.AsEnumerable();
        }
    }

    public class ItemDeliveryQuest_loadQuestInfo_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling ItemDeliveryQuest.loadQuestInfo");

            var codes = new List<CodeInstruction>(instructions);

            bool start = false;
            bool found1 = false;
            bool found2 = false;
            for (int i = 0; i < codes.Count; i++)
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
                            codes.Insert(i + 1,
                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetPossibleCrops))));
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
}