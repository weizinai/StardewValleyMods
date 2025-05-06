using HarmonyLib;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class GameLocationPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        // harmony.Patch(
        //     original: this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction)),
        //     prefix: this.GetHarmonyMethod(nameof(AnswerDialogueActionPrefix))
        // );
        // harmony.Patch(
        //     original: this.RequireMethod<GameLocation>("houseUpgradeAccept"),
        //     prefix: this.GetHarmonyMethod(nameof(HouseUpgradeAcceptPrefix))
        // );
    }

    // 禁止购买背包
    // private static bool AnswerDialogueActionPrefix(string questionAndAnswer)
    // {
    //     if (!SpendLimitHelper.IsSpendLimitEnable()) return true;
    //
    //     if (questionAndAnswer == "Backpack_Purchase")
    //     {
    //         var player = Game1.player;
    //         SpendLimitHelper.GetFarmerSpendData(out var amount, out _, out var availableMoney);
    //         switch (player.MaxItems)
    //         {
    //             case 12:
    //                 if (availableMoney < 2000)
    //                 {
    //                     SpendLimitHelper.ShowSpendLimitDialogue("购买大背包", 2000);
    //                     return false;
    //                 }
    //                 if (player.Money >= 2000)
    //                 {
    //                     player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 2000).ToString();
    //                 }
    //                 break;
    //             case 24:
    //                 if (availableMoney < 10000)
    //                 {
    //                     SpendLimitHelper.ShowSpendLimitDialogue("购买豪华背包", 10000);
    //                     return false;
    //                 }
    //                 if (player.Money >= 10000)
    //                 {
    //                     player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 10000).ToString();
    //                 }
    //                 break;
    //         }
    //     }
    //
    //     return true;
    // }

    // 禁止房屋升级
    // private static bool HouseUpgradeAcceptPrefix()
    // {
    //     if (!SpendLimitHelper.IsSpendLimitEnable()) return true;
    //
    //     var player = Game1.player;
    //     SpendLimitHelper.GetFarmerSpendData(out var amount, out _, out var availableMoney);
    //     switch (player.HouseUpgradeLevel)
    //     {
    //         case 0:
    //             if (availableMoney < 10000)
    //             {
    //                 SpendLimitHelper.ShowSpendLimitDialogue("升级1级房子", 10000);
    //                 return false;
    //             }
    //             if (player.Money >= 10000 && player.Items.ContainsId("(O)388", 450))
    //             {
    //                 player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 10000).ToString();
    //             }
    //             break;
    //         case 1:
    //             if (availableMoney < 65000)
    //             {
    //                 SpendLimitHelper.ShowSpendLimitDialogue("升级2级房子", 65000);
    //                 return false;
    //             }
    //             if (player.Money >= 65000 && player.Items.ContainsId("(O)709", 100))
    //             {
    //                 player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 65000).ToString();
    //             }
    //             break;
    //         case 2:
    //             if (availableMoney < 100000)
    //             {
    //                 SpendLimitHelper.ShowSpendLimitDialogue("升级3级房子", 100000);
    //                 return false;
    //             }
    //             if (player.Money >= 100000)
    //             {
    //                 player.modData[SpendLimitHandler.SpendAmountKey] = (amount + 100000).ToString();
    //             }
    //             break;
    //     }
    //
    //     return true;
    // }
}