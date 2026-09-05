using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.QuestBuilder;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class ItemDeliveryQuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.loadQuestInfo)),
            prefix: this.GetHarmonyMethod(nameof(LoadQuestInfoPrefix))
        );
        harmony.Patch(
            original: this.RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.OnItemOfferedToNpc)),
            transpiler: this.GetHarmonyMethod(nameof(OnItemOfferedToNpcTranspiler))
        );
    }

    private static bool LoadQuestInfoPrefix(ItemDeliveryQuest __instance)
    {
        var builder = new ItemDeliveryQuestBuilder(__instance);

        builder.BuildQuest();

        return false;
    }

    // 交易任务友谊奖励修改
    private static IEnumerable<CodeInstruction> OnItemOfferedToNpcTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldc_I4, 150));

        if (!codeMatcher.IsValid)
        {
            Logger.Error("Target instruction not found [Opcode: Ldc_I4, Operand: 150]");

            return codeMatcher.Instructions();
        }

        codeMatcher.SetInstruction(CodeInstruction.Call(typeof(ItemDeliveryQuestPatcher), nameof(GetItemDeliveryFriendshipGain)));

        return codeMatcher.Instructions();
    }

    private static int GetItemDeliveryFriendshipGain()
    {
        return ModConfig.Instance.VanillaConfig.ItemDeliveryFriendshipGain;
    }
}