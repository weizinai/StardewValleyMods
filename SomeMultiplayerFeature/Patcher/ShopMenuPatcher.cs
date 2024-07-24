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
        if (!CanBuyItem(__instance, item)) return true;

        var stockInfo = __instance.itemPriceAndStock[item];
        var totalPrice = stockInfo.Price * stockToBuy;
        var player = Game1.player;
        GetTradeItemData(stockInfo, out var tradeItem, out var tradeItemCount);

        if (player.Money >= totalPrice &&
            (tradeItem == null || __instance.HasTradeItem(tradeItem, tradeItemCount * stockToBuy)))
        {
            if (SpendLimitHelper.IsSpendLimitEnable())
            {
                SpendLimitHelper.GetFarmerSpendData(out var amount, out _, out var availableMoney);
                if (availableMoney < totalPrice)
                {
                    SpendLimitHelper.ShowSpendLimitDialogue($"购买{stockToBuy}个{item.DisplayName}", totalPrice);
                    return false;
                }
                player.modData[SpendLimitHandler.SpendAmountKey] = (amount + totalPrice).ToString();
            }

            __state = true;
        }

        return true;
    }

    private static void TryToPurchaseItemPostfix(ISalable item, int stockToBuy, bool __state)
    {
        if (__state) MultiplayerLog.NoIconHUDMessage($"{Game1.player.Name}购买了 {stockToBuy} 个{item.DisplayName}", 500);
    }

    private static bool CanBuyItem(ShopMenu menu, ISalable item)
    {
        return menu.currency == 0 &&                                          // 商店货币为金币
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