using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Quests;
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
        var codes = new List<CodeInstruction>(instructions);
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4 && code.operand.Equals(150));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemDeliveryQuestPatcher), nameof(GetItemDeliveryFriendshipGain)));
        return codes.AsEnumerable();
    }

    private static int GetItemDeliveryFriendshipGain()
    {
        return ModConfig.Instance.VanillaConfig.ItemDeliveryFriendshipGain;
    }
}