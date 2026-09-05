using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

public class QuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Quest>(nameof(Quest.IsTimedQuest)),
            postfix: this.GetHarmonyMethod(nameof(IsTimedQuestPostfix))
        );

        harmony.Patch(
            original: this.RequireMethod<Quest>(nameof(Quest.getQuestFromId)),
            postfix: this.GetHarmonyMethod(nameof(GetQuestFromIdPostfix))
        );
    }

    // 使每日任务的时间可以没有限制
    private static void IsTimedQuestPostfix(Quest __instance, ref bool __result)
    {
        __result = __instance.GetDaysLeft() > 0;
    }

    // 修复剧情任务有时间限制的问题
    private static void GetQuestFromIdPostfix(Quest __result)
    {
        __result.daysLeft.Value = 0;
    }
}