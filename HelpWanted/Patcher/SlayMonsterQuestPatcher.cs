using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.QuestBuilder;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class SlayMonsterQuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<SlayMonsterQuest>(nameof(SlayMonsterQuest.loadQuestInfo)),
            prefix: this.GetHarmonyMethod(nameof(LoadQuestInfoPrefix))
        );
    }

    private static bool LoadQuestInfoPrefix(SlayMonsterQuest __instance)
    {
        var builder = new SlayMonsterQuestBuilder(__instance);

        builder.BuildQuest();

        return false;
    }
}