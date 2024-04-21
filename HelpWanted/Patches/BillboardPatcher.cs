using System.Reflection.Metadata.Ecma335;
using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace HelpWanted.Patches;

public class BillboardPatcher : BasePatcher
{
    public override void Patch(Harmony harmony, IMonitor monitor)
    {
        harmony.Patch(
            RequireMethod<Billboard>(nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
            GetHarmonyMethod(nameof(DrawPrefix))
        );
    }
    
    private static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        if (!___dailyQuestBoard || Game1.activeClickableMenu.GetType() != typeof(Billboard))
            return true;
        Game1.activeClickableMenu = new HWQuestBoard();
        return false;
    }

    private static void ReceiveLeftClickPostfix(Billboard __instance, bool ___dailyQuestBoard, int x, int y)
    {
        var config = ModEntry.Config;
        if (!___dailyQuestBoard || Game1.activeClickableMenu is not HWQuestBoard)
            return;
        __instance.acceptQuestButton.visible = true;
        if (__instance.acceptQuestButton.containsPoint(x, y))
        {
            Game1.questOfTheDay.daysLeft.Value = config.QuestDays;
            Game1.player.acceptedDailyQuest.Set(false);
            Game1.netWorldState.Value.SetQuestOfTheDay(null);
            HWQuestBoard.QuestDataDictionary.Remove(HWQuestBoard.ShowingQuestID);
            HWQuestBoard.QuestNotes.RemoveAll(option => option.myID == HWQuestBoard.ShowingQuestID);
            HWQuestBoard.ShowingQuest = null;
        }
        else if (__instance.upperRightCloseButton.containsPoint(x, y))
        {
            HWQuestBoard.ShowingQuest = null;
        }
    }
}