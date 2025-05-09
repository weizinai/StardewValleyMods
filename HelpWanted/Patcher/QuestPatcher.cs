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
    }
    
    // 使每日任务的时间可以没有限制
    private static void IsTimedQuestPostfix(Quest __instance, ref bool __result)
    {
        __result = __instance.GetDaysLeft() > 0;
    }
}