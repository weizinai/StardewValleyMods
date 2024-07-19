using System.Text.Json;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
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
        if (Game1.IsServer) return true;

        if (__instance.itemPriceAndStock[item].TradeItem != null) return true;

        var player = Game1.player;

        if (!TryGetPlayerLimit(player, out var limit, out var amount)) return true;

        var availableMoney = limit - amount;

        if (availableMoney < item.salePrice() * stockToBuy)
        {
            Game1.drawObjectDialogue(
                $"你今日的消费金额为{amount}，总消费额度为{limit}，可用额度为{availableMoney}。购买{stockToBuy}个{item.DisplayName}需要{item.salePrice() * stockToBuy}金币，超过你的可用额度{availableMoney}。因此你的购物行为被禁止。");
            stockToBuy = 0;
            return false;
        }

        player.modData[SpendLimitHandler.SpentAmountKey] = (amount + item.salePrice() * stockToBuy).ToString();
        return true;
    }

    private static void TryToPurchaseItemPostfix(ISalable item, int stockToBuy)
    {
        if (stockToBuy > 0) MultiplayerLog.NoIconHUDMessage($"{Game1.player.Name}购买了 {stockToBuy} 个{item.DisplayName}", 500);
    }

    private static bool TryGetPlayerLimit(Farmer player, out int limit, out int amount)
    {
        Game1.MasterPlayer.modData.TryGetValue(SpendLimitHandler.SpentLimitDataKey, out var rawLimit);
        player.modData.TryGetValue(SpendLimitHandler.SpentAmountKey, out var rawAmount);

        if (rawLimit == null || rawAmount == null)
        {
            limit = 0;
            amount = 0;
            return false;
        }

        limit = JsonSerializer.Deserialize<Dictionary<string, int>>(rawLimit)![player.Name];
        amount = int.Parse(rawAmount);

        return true;
    }
}