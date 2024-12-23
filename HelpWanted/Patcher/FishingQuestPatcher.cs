﻿using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class FishingQuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<FishingQuest>(nameof(FishingQuest.loadQuestInfo)),
            transpiler: this.GetHarmonyMethod(nameof(LoadQuestInfoTranspiler))
        );
    }

    // 钓鱼任务奖励修改
    private static IEnumerable<CodeInstruction> LoadQuestInfoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Mul);
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FishingQuestPatcher), nameof(GetReward))));
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Mul);
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FishingQuestPatcher), nameof(GetReward))));

        return codes.AsEnumerable();
    }

    private static int GetReward(int reward)
    {
        return (int)(reward * ModConfig.Instance.FishingRewardMultiplier);
    }
}