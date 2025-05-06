using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.QuestBuilder;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class FishingQuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<FishingQuest>(nameof(FishingQuest.loadQuestInfo)),
            prefix: this.GetHarmonyMethod(nameof(LoadQuestInfoPrefix))
        );
    }

    private static bool LoadQuestInfoPrefix(FishingQuest __instance)
    {
        var builder = new FishingQuestBuilder(__instance);

        builder.BuildQuest();

        return false;
    }
}