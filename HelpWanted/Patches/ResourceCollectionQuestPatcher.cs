using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

internal class ResourceCollectionQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static bool hasLoadQuestInfo;

    public ResourceCollectionQuestPatcher(ModConfig config)
    {
        ResourceCollectionQuestPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<ResourceCollectionQuest>(nameof(ResourceCollectionQuest.loadQuestInfo)),
            GetHarmonyMethod(nameof(LoadQuestInfoPrefix)),
            GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }
    
    private static bool LoadQuestInfoPrefix(ResourceCollectionQuest __instance)
    {
        if (__instance.target.Value is not null && __instance.ItemId.Value is not null)
        {
            hasLoadQuestInfo = false;
            return false;
        }

        hasLoadQuestInfo = true;
        return true;
    }
    
    private static void LoadQuestInfoPostfix(ref NetInt ___reward, ref NetDescriptionElementList ___parts)
    {
        if (hasLoadQuestInfo) return;
        ___reward.Value = (int)(___reward.Value * config.ResourceCollectionRewardMultiplier);
        ___parts[^2].substitutions = new List<object> { ___reward.Value };
    }
}