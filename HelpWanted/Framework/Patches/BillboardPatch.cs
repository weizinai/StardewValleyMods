using StardewValley;
using StardewValley.Menus;

namespace HelpWanted.Framework.Patches;

public class BillboardPatch
{
    public static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        var config = ModEntry.Config;
        if (!config.ModEnabled || !___dailyQuestBoard || Game1.activeClickableMenu.GetType() != typeof(Billboard))
            return true;
        Game1.activeClickableMenu = new HWQuestBoard();
        return false;
    }

    public static void ReceiveLeftClickPostfix(Billboard __instance, bool ___dailyQuestBoard, int x, int y)
    {
        var config = ModEntry.Config;
        if (!config.ModEnabled || !___dailyQuestBoard || Game1.activeClickableMenu is not HWQuestBoard)
            return;
        __instance.acceptQuestButton.visible = true;
        if (__instance.acceptQuestButton.containsPoint(x, y))
        {
            Game1.questOfTheDay.daysLeft.Value = config.ModEnabled ? config.QuestDays : 2;
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