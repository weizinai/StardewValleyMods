using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

public class ResourceCollectionQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public ResourceCollectionQuestPatcher(ModConfig config)
    {
        ResourceCollectionQuestPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<ResourceCollectionQuest>(nameof(ResourceCollectionQuest.loadQuestInfo)),
            postfix: GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }
    
    private static void LoadQuestInfoPostfix(ResourceCollectionQuest __instance , ref NetInt ___reward, ref NetDescriptionElementList ___parts)
    {
        if (__instance.target.Value is not null && __instance.ItemId.Value is not null) return;
        
        ___reward.Value = (int)(___reward.Value * config.ResourceCollectionRewardModifier);
        ___parts[^2].substitutions = new List<object> { ___reward.Value };
    }
}