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

    private static void IsTimedQuestPostfix(Quest __instance, ref bool __result)
    {
        __result = __instance.GetDaysLeft() > 0;
    }
}