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
    private static bool TryToPurchaseItemPrefix(ISalable item, ref int stockToBuy, ShopMenu __instance, ref bool __state)
    {
        if (!CheckSpendLimitEnable(__instance, item)) return true;

        SpendLimitHelper.GetFarmerSpendData(out var amount, out var limit, out var availableMoney);
        var stockInfo = __instance.itemPriceAndStock[item];
        var totalPrice = stockInfo.Price * stockToBuy;
        if (availableMoney < totalPrice)
        {
            var dialogues = new List<string>
            {
                $"当日消费：{amount}金|可用额度：{availableMoney}金|总额度：{limit}金",
                $"购买{stockToBuy}个{item.DisplayName}需要{totalPrice}金，超过可用额度{totalPrice - availableMoney}金"
            };
            Game1.drawObjectDialogue(dialogues);
            stockToBuy = 0;
            return false;
        }

        GetTradeItemData(stockInfo, out var tradeItem, out var tradeItemCount);
        var player = Game1.player;
        if (player.Money >= totalPrice &&
            (tradeItem == null || __instance.HasTradeItem(tradeItem, tradeItemCount * stockToBuy)))
        {
            player.modData[SpendLimitHandler.SpendAmountKey] = (amount + totalPrice).ToString();
            __state = true;
        }

        return true;
    }

    private static void TryToPurchaseItemPostfix(ISalable item, int stockToBuy, bool __state)
    {
        if (__state) MultiplayerLog.NoIconHUDMessage($"{Game1.player.Name}购买了 {stockToBuy} 个{item.DisplayName}", 500);
    }

    private static bool CheckSpendLimitEnable(ShopMenu menu, ISalable item)
    {
        return SpendLimitHelper.IsSpendLimitEnable() &&                       // 花钱限制功能是否启用
               menu.currency == 0 &&                                          // 商店货币为金币
               (menu.heldItem == null || menu.heldItem.canStackWith(item)) && // 当前未持有物品或者持有的物品能与要购买的物品堆叠
               Game1.player.couldInventoryAcceptThisItem(item as Item);       // 玩家有足够的空间容纳要购买的物品
    }

    private static void GetTradeItemData(ItemStockInformation stockInfo, out string? tradeItem, out int tradeItemCount)
    {
        tradeItem = null;
        tradeItemCount = 5;
        if (stockInfo.TradeItem != null)
        {
            tradeItem = stockInfo.TradeItem;
            if (stockInfo.TradeItemCount.HasValue)
            {
                tradeItemCount = stockInfo.TradeItemCount.Value;
            }
        }
    }
}