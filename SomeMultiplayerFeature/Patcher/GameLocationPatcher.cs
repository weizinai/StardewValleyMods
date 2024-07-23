using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class GameLocationPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<GameLocation>("breakStone"),
            transpiler: this.GetHarmonyMethod(nameof(BreakStoneTranspiler))
        );
        harmony.Patch(
            original: this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction)),
            prefix: this.GetHarmonyMethod(nameof(AnswerDialogueActionPrefix))
        );
        harmony.Patch(
            original: this.RequireMethod<GameLocation>("houseUpgradeAccept"),
            prefix: this.GetHarmonyMethod(nameof(HouseUpgradeAcceptPrefix))
        );
    }

    // 采集铜矿、铁矿、金矿和铱矿分别获得11点、12点、13点和14点采矿经验
    private static IEnumerable<CodeInstruction> BreakStoneTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindLastIndex(code => code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)))) - 1;
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationPatcher), nameof(GetStoneExperience))));

        Log.Info("\n修改采矿经验：\n铜矿：11点\n铁矿：12点\n金矿：13点\n铱矿：14点");

        return codes.AsEnumerable();
    }

    // 禁止购买背包
    private static bool AnswerDialogueActionPrefix(string questionAndAnswer)
    {
        if (!SpendLimitHelper.IsSpendLimitEnable()) return true;

        if (questionAndAnswer == "Backpack_Purchase")
        {
            var player = Game1.player;
            SpendLimitHelper.GetFarmerSpendData(out var amount, out _, out var availableMoney);
            switch (player.MaxItems)
            {
                case 12:
                    if (availableMoney < 2000)
                    {
                        SpendLimitHelper.ShowSpendLimitDialogue("购买大背包", 2000);
                        return false;
                    }
                    if (player.Money >= 2000)
                    {
                        player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 2000).ToString();
                    }
                    break;
                case 24:
                    if (availableMoney < 10000)
                    {
                        SpendLimitHelper.ShowSpendLimitDialogue("购买豪华背包", 10000);
                        return false;
                    }
                    if (player.Money >= 10000)
                    {
                        player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 10000).ToString();
                    }
                    break;
            }
        }

        return true;
    }

    // 禁止房屋升级
    private static bool HouseUpgradeAcceptPrefix()
    {
        if (!SpendLimitHelper.IsSpendLimitEnable()) return true;

        var player = Game1.player;
        SpendLimitHelper.GetFarmerSpendData(out var amount, out _, out var availableMoney);
        switch (player.HouseUpgradeLevel)
        {
            case 0:
                if (availableMoney < 10000)
                {
                    SpendLimitHelper.ShowSpendLimitDialogue("升级1级房子", 10000);
                    return false;
                }
                if (player.Money >= 10000 && player.Items.ContainsId("(O)388", 450))
                {
                    player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 10000).ToString();
                }
                break;
            case 1:
                if (availableMoney < 65000)
                {
                    SpendLimitHelper.ShowSpendLimitDialogue("升级2级房子", 65000);
                    return false;
                }
                if (player.Money >= 65000 && player.Items.ContainsId("(O)709", 100))
                {
                    player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 65000).ToString();
                }
                break;
            case 2:
                if (availableMoney < 100000)
                {
                    SpendLimitHelper.ShowSpendLimitDialogue("升级3级房子", 100000);
                    return false;
                }
                if (player.Money >= 100000)
                {
                    player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 100000).ToString();
                }
                break;
        }

        return true;
    }

    private static int GetStoneExperience(int origin, string stoneId)
    {
        origin = stoneId switch
        {
            "751" => 10, // 铜矿
            "290" => 11, // 铁矿
            "764" => 12, // 金矿
            "765" => 13, // 铱矿
            _ => origin
        };

        return origin;
    }
}