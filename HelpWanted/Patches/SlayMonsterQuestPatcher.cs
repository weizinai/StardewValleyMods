using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

public class SlayMonsterQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static bool hasLoadQuestInfo;

    public SlayMonsterQuestPatcher(ModConfig config)
    {
        SlayMonsterQuestPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<SlayMonsterQuest>(nameof(SlayMonsterQuest.loadQuestInfo)),
            prefix: GetHarmonyMethod(nameof(LoadQuestInfoPrefix)),
            postfix: GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }
    
    private static bool LoadQuestInfoPrefix(SlayMonsterQuest __instance)
    {
        if (__instance.target.Value is not null && __instance.monster is not null)
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
        
        ___reward.Value = (int)(___reward.Value * config.SlayMonstersRewardMultiplier);
        ___parts[^1].substitutions = new List<object> { ___reward.Value };
    }
    
}