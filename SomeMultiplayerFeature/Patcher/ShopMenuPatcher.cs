using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class ShopMenuPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<ShopMenu>("tryToPurchaseItem"),
            prefix: this.GetHarmonyMethod(nameof(TryToPurchaseItemPrefix)),
            postfix: this.GetHarmonyMethod(nameof(TryToPurchaseItemPostfix))
        );
    }

    // 购物限制
    private static bool TryToPurchaseItemPrefix(ISalable item, ref int stockToBuy, ShopMenu __instance)
    {
        if (!CheckSpendLimitEnable(__instance.itemPriceAndStock[item])) return true;

        var player = Game1.player;
        SpendLimitHelper.TryGetFarmerSpendLimit(player.Name, out var limit);
        var amount = int.Parse(player.modData[SpendLimitHandler.SpendAmountKey]);
        var availableMoney = limit - amount;
        var totalPrice = item.salePrice() * stockToBuy;
        if (availableMoney < totalPrice)
        {
            var dialogues = new List<string>()
            {
                $"当日消费：{amount}金|可用额度：{availableMoney}金|总额度：{limit}金",
                $"购买{stockToBuy}个{item.DisplayName}需要{totalPrice}金，超过可用额度{totalPrice - availableMoney}金"
            };
            Game1.drawObjectDialogue(dialogues);
            stockToBuy = 0;
            return false;
        }

        player.modData[SpendLimitHandler.SpendAmountKey] = (amount + item.salePrice() * stockToBuy).ToString();
        return true;
    }

    private static void TryToPurchaseItemPostfix(ISalable item, int stockToBuy)
    {
        if (stockToBuy > 0) MultiplayerLog.NoIconHUDMessage($"{Game1.player.Name}购买了 {stockToBuy} 个{item.DisplayName}", 500);
    }

    private static bool CheckSpendLimitEnable(ItemStockInformation itemStockInformation)
    {
        return !Game1.IsServer &&
               Game1.MasterPlayer.modData.ContainsKey(SpendLimitHandler.SpendLimitKey) &&
               itemStockInformation.TradeItem == null;
    }
}