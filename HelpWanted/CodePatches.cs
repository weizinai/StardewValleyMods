using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HelpWanted.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Quests;
using SObject = StardewValley.Object;
// ReSharper disable InconsistentNaming

namespace HelpWanted;

internal partial class ModEntry
{
    private void HarmonyPatch()
    {
        var harmony = new Harmony("aedenthorn.HelpWanted");
        harmony.Patch(
            AccessTools.Method(typeof(Game1), "CanAcceptDailyQuest"),
            new HarmonyMethod(typeof(Game1_CanAcceptDailyQuest_Patch), "Prefix")
        );
        harmony.Patch(
            AccessTools.Method(typeof(DescriptionElement), "loadDescriptionElement"),
            new HarmonyMethod(typeof(DescriptionElement_loadDescriptionElement_Patch), "Prefix")
        );
        harmony.Patch(
            AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
            new HarmonyMethod(typeof(Billboard_draw_Patch), "Prefix")
        );
        /*
        harmony.Patch(
            AccessTools.Method(typeof(Billboard_receiveLeftClick_Patch), "receiveLeftClick"),
            postfix: new HarmonyMethod(typeof(Billboard_receiveLeftClick_Patch), "Postfix")
        );
        */
    }

    public class Game1_CanAcceptDailyQuest_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            // 如果模组未启用，则执行原逻辑
            if (!Config.ModEnabled)
                return true;
            // 如果模组启用
            try
            {
                __result = Game1.questOfTheDay != null && !Game1.player.acceptedDailyQuest.Value &&
                           !string.IsNullOrEmpty(Game1.questOfTheDay.questDescription);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }

    public class DescriptionElement_loadDescriptionElement_Patch
    {
        public static bool Prefix(DescriptionElement __instance, ref string __result)
        {
            // 如果模组未启用，则执行原逻辑
            if (!Config.ModEnabled)
                return true;
            // 如果模组启用
            try
            {
                var temp = new DescriptionElement(__instance.translationKey, __instance.substitutions);
                for (var i = 0; i < temp.substitutions.Count; i++)
                    switch (temp.substitutions[i])
                    {
                        case DescriptionElement descriptionElement1:
                            temp.substitutions[i] = descriptionElement1.loadDescriptionElement();
                            break;
                        case SObject sObject:
                            temp.substitutions[i] = ItemRegistry.GetDataOrErrorItem(sObject.QualifiedItemId).DisplayName;
                            break;
                        case Monster monster:
                            DescriptionElement descriptionElement2;
                            if (monster.Name == "Frost Jelly")
                            {
                                descriptionElement2 = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13772",
                                    Array.Empty<object>());
                                temp.substitutions[i] = descriptionElement2.loadDescriptionElement();
                            }
                            else
                            {
                                descriptionElement2 = new DescriptionElement("Data\\Monsters:" + monster.Name);
                                temp.substitutions[i] = LocalizedContentManager.CurrentLanguageCode ==
                                                        LocalizedContentManager.LanguageCode.en
                                    ? descriptionElement2.loadDescriptionElement().Split('/').Last() + "s"
                                    : descriptionElement2.loadDescriptionElement().Split('/').Last();
                            }

                            temp.substitutions[i] = descriptionElement2.loadDescriptionElement().Split('/').Last();
                            break;
                        case NPC npc:
                            temp.substitutions[i] = NPC.GetDisplayName(npc.Name);
                            break;
                    }

                /*
                for (var i = 0; i < temp.substitutions.Count; i++)
                {
                    if (temp.substitutions[i] is DescriptionElement)
                    {
                        var d = temp.substitutions[i] as DescriptionElement;
                        temp.substitutions[i] = d.loadDescriptionElement();
                    }

                    if (temp.substitutions[i] is SObject)
                        temp.substitutions[i] = ItemRegistry.GetDataOrErrorItem((temp.substitutions[i] as SObject).QualifiedItemId)
                            .DisplayName;
                    // string objectInformation;
                    // Game1.objectData.TryGetValue((temp.substitutions[i] as Object).ParentSheetIndex, out objectInformation);
                    // temp.substitutions[i] = objectInformation.Split('/', StringSplitOptions.None)[4];
                    if (temp.substitutions[i] is Monster)
                    {
                        DescriptionElement d2;
                        if ((temp.substitutions[i] as Monster).Name.Equals("Frost Jelly"))
                        {
                            d2 = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13772");
                            temp.substitutions[i] = d2.loadDescriptionElement();
                        }
                        else
                        {
                            d2 = new DescriptionElement("Data\\Monsters:" + (temp.substitutions[i] as Monster).Name);
                            temp.substitutions[i] =
                                LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en
                                    ? d2.loadDescriptionElement().Split('/').Last() + "s"
                                    : d2.loadDescriptionElement().Split('/').Last();
                        }

                        temp.substitutions[i] = d2.loadDescriptionElement().Split('/').Last();
                    }

                    if (temp.substitutions[i] is NPC)
                    {
                        var d3 = new DescriptionElement("Data\\NPCDispositions:" + (temp.substitutions[i] as NPC).Name);
                        temp.substitutions[i] = d3.loadDescriptionElement().Split('/').Last();
                    }
                }
                */

                return true;
            }
            catch
            {
                __result = string.Empty;
                return false;
            }
        }
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
                Game1.player.acceptedDailyQuest.Set(false);
                Game1.netWorldState.Value.SetQuestOfTheDay(null);
                OrdersBillboard.QuestDataDictionary.Remove(OrdersBillboard.ShowingQuest);
                OrdersBillboard.QuestOptions.RemoveAll(c => c.myID == OrdersBillboard.ShowingQuest);
                OrdersBillboard.QuestBillboard = null;
            }
            else if (__instance.upperRightCloseButton.containsPoint(x, y))
            {
                OrdersBillboard.QuestBillboard = null;
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Billboard.receiveLeftClick");

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (i < codes.Count - 1 && codes[i].opcode == OpCodes.Ldfld && codes[i + 1].opcode == OpCodes.Ldc_I4_2 &&
                    (FieldInfo)codes[i].operand == AccessTools.Field(typeof(Quest), nameof(Quest.daysLeft)))
                {
                    SMonitor.Log($"replacing days left with method");
                    codes.Insert(i + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetQuestDays))));
                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }

    public static int GetQuestDays(int days)
    {
        return !Config.ModEnabled ? days : Config.QuestDays;
    }
    /*
    public class Utility_getRandomItemFromSeason_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Utility.getRandomItemFromSeason");

            var codes = new List<CodeInstruction>(instructions);
            codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Ldloc_1));
            codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetRandomItem))));

            return codes.AsEnumerable();
        }
        public static void Prefix(ref int randomSeedAddition)
        {
            if (!Config.ModEnabled || !gettingQuestDetails)
                return;
            randomSeedAddition += Random.Next();

        }
    }
    */
}