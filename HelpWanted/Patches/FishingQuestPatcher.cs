using System.Reflection.Emit;
using Common.Patcher;
using HarmonyLib;
using HelpWanted.Framework;
using StardewValley.Quests;

namespace HelpWanted.Patches;

internal class FishingQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public FishingQuestPatcher(ModConfig config)
    {
        FishingQuestPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<FishingQuest>(nameof(FishingQuest.loadQuestInfo)),
            transpiler: GetHarmonyMethod(nameof(LoadQuestInfoTranspiler))
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
        return (int)(reward * config.FishingRewardMultiplier);
    }
}