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
        if (Game1.IsServer) return true;

        if (questionAndAnswer == "Backpack_Purchase")
        {
            var player = Game1.player;
            SpendLimitHelper.GetFarmerSpendData(out var amount, out var limit, out var availableMoney);
            switch (player.MaxItems)
            {
                case 12:
                    if (availableMoney < 2000)
                    {
                        var dialogues = new List<string>
                        {
                            $"当日消费：{amount}金|可用额度：{availableMoney}金|总额度：{limit}金",
                            $"购买大背包需要2000金，超过可用额度{2000 - availableMoney}金"
                        };
                        Game1.drawObjectDialogue(dialogues);
                        return false;
                    }
                    if (player.Money >= 2000)
                    {
                        player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 2000).ToString();
                        MultiplayerLog.NoIconHUDMessage($"{player.Name}购买了大背包", 500);
                    }
                    break;
                case 24:
                    if (availableMoney < 10000)
                    {
                        var dialogues = new List<string>
                        {
                            $"当日消费：{amount}金|可用额度：{availableMoney}金|总额度：{limit}金",
                            $"购买大背包需要10000金，超过可用额度{10000 - availableMoney}金"
                        };
                        Game1.drawObjectDialogue(dialogues);
                        return false;
                    }
                    if (player.Money >= 10000)
                    {
                        player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 10000).ToString();
                        MultiplayerLog.NoIconHUDMessage($"{player.Name}购买了豪华背包", 500);
                    }
                    break;
            }
        }

        return true;
    }

    // 禁止房屋升级
    private static bool HouseUpgradeAcceptPrefix()
    {
        if (Game1.IsServer) return true;

        var modData = Game1.MasterPlayer.modData;
        var level = Game1.player.HouseUpgradeLevel;
        if (modData.ContainsKey(HouseUpgradeHandler.HouseUpgradeKey + level))
        {
            Game1.drawObjectDialogue("房屋升级已被禁止");
            return false;
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