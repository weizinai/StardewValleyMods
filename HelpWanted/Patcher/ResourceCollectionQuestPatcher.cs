﻿using System.Reflection.Emit;
using HarmonyLib;
using Netcode;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class ResourceCollectionQuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<ResourceCollectionQuest>(nameof(ResourceCollectionQuest.loadQuestInfo)),
            transpiler: this.GetHarmonyMethod(nameof(LoadQuestInfoTranspiler))
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
        return (int)(reward.Value * ModConfig.Instance.ResourceCollectionRewardMultiplier);
    }
}