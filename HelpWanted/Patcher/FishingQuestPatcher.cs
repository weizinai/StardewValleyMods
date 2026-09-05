using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.QuestBuilder;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class FishingQuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<FishingQuest>(harmony, nameof(FishingQuest.loadQuestInfo), PatchKind.Prefix, nameof(LoadQuestInfoPrefix));
    }

    private static bool LoadQuestInfoPrefix(FishingQuest __instance)
    {
        var builder = new FishingQuestBuilder(__instance);

        builder.BuildQuest();

        return false;
    }
}
