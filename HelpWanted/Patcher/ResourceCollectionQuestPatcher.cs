using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.QuestBuilder;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class ResourceCollectionQuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<ResourceCollectionQuest>(harmony, nameof(ResourceCollectionQuest.loadQuestInfo), PatchKind.Prefix, nameof(LoadQuestInfoPrefix));
    }

    private static bool LoadQuestInfoPrefix(ResourceCollectionQuest __instance)
    {
        var builder = new ResourceCollectionQuestBuilder(__instance);

        builder.BuildQuest();

        return false;
    }
}
