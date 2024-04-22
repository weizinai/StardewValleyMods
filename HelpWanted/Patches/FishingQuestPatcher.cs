using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

public class FishingQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public FishingQuestPatcher(ModConfig config)
    {
        FishingQuestPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<FishingQuest>(nameof(FishingQuest.loadQuestInfo)),
            postfix: GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }

    private static void LoadQuestInfoPostfix(FishingQuest __instance, NetInt ___reward, ref NetDescriptionElementList ___parts)
    {
        if (__instance.target.Value is not null && __instance.ItemId.Value is not null) return;
        
        ___reward.Value = (int)(___reward.Value * config.FishingRewardModifier);
        ___parts[^2].substitutions = new List<object> { ___reward.Value };
        if (__instance.target.Value is "Willy") ___parts[^3].substitutions[0] = ___reward.Value;
    }
}