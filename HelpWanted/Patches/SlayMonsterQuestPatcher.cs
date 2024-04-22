﻿using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

public class SlayMonsterQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public SlayMonsterQuestPatcher(ModConfig config)
    {
        SlayMonsterQuestPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<SlayMonsterQuest>(nameof(SlayMonsterQuest.loadQuestInfo)),
            postfix: GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }

    private static void LoadQuestInfoPostfix(FishingQuest __instance, NetInt ___reward, ref NetDescriptionElementList ___parts)
    {
        if (__instance.target.Value is not null && __instance.ItemId.Value is not null) return;
        
        ___reward.Value = (int)(___reward.Value * config.SlayMonstersRewardModifier);
        ___parts[^1].substitutions = new List<object> { ___reward.Value };
    }
    
}