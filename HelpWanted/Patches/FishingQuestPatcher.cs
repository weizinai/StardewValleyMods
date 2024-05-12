using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

internal class FishingQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static bool hasLoadQuestInfo;

    public FishingQuestPatcher(ModConfig config)
    {
        FishingQuestPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<FishingQuest>(nameof(FishingQuest.loadQuestInfo)),
            GetHarmonyMethod(nameof(LoadQuestInfoPrefix)),
            GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }

    private static bool LoadQuestInfoPrefix(FishingQuest __instance)
    {
        if (__instance.target.Value is not null && __instance.ItemId.Value is not null)
        {
            hasLoadQuestInfo = false;
            return false;
        }

        hasLoadQuestInfo = true;
        return true;
    }

    private static void LoadQuestInfoPostfix(NetInt ___reward, ref NetDescriptionElementList ___parts)
    {
        if (hasLoadQuestInfo) return;

        ___reward.Value = (int)(___reward.Value * config.FishingRewardMultiplier);
        var keySet = new HashSet<string> { "Strings\\StringsFromCSFiles:FishingQuest.cs.13248", "Strings\\StringsFromCSFiles:FishingQuest.cs.13274" };
        foreach (var part in ___parts.Where(part => keySet.Contains(part.translationKey))) part.substitutions[0] = ___reward.Value;
    }
}