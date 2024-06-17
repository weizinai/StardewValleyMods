﻿using System.Reflection.Emit;
using Common.Patcher;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

internal class ResourceCollectionQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public ResourceCollectionQuestPatcher(ModConfig config)
    {
        ResourceCollectionQuestPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<ResourceCollectionQuest>(nameof(ResourceCollectionQuest.loadQuestInfo)),
            transpiler: GetHarmonyMethod(nameof(LoadQuestInfoTranspiler))
        );
    }

    // 任务奖励修改逻辑
    private static IEnumerable<CodeInstruction> LoadQuestInfoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Stloc_3);
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_0));
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ResourceCollectionQuest), nameof(ResourceCollectionQuest.reward))));
        codes.Insert(index + 3, new CodeInstruction(OpCodes.Ldarg_0));
        codes.Insert(index + 4, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ResourceCollectionQuest), nameof(ResourceCollectionQuest.reward))));
        codes.Insert(index + 5, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ResourceCollectionQuestPatcher), nameof(GetReward))));
        codes.Insert(index + 6, new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetInt), nameof(NetInt.Set))));

        return codes.AsEnumerable();
    }

    private static int GetReward(NetInt reward)
    {
        return (int)(reward.Value * config.ResourceCollectionRewardMultiplier);
    }
}