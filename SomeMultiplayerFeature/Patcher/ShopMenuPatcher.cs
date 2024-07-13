using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class ShopMenuPatcher : BasePatcher
{
    private const string FreezeMoneyKey = "weizinai.SomeMultiplayerFeature_FreezeMoney";

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<ShopMenu>("tryToPurchaseItem"),
            prefix: this.GetHarmonyMethod(nameof(TryToPurchaseItemPrefix)),
            postfix: this.GetHarmonyMethod(nameof(TryToPurchaseItemPostfix))
        );
    }

    // 冻结金钱
    private static bool TryToPurchaseItemPrefix(ISalable item, int stockToBuy)
    {
        if (Game1.IsServer) return true;

        Game1.MasterPlayer.modData.TryGetValue(FreezeMoneyKey, out var value);
        if (value == null) return true;

        var player = Game1.player;
        var freezeMoney = int.Parse(value);
        if (player.Money <= freezeMoney + item.salePrice() * stockToBuy)
        {
            Game1.drawObjectDialogue($"主机冻结了{freezeMoney}金，你无法使用金钱");
            Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode,
                $"{player.Name}想购买{item.DisplayName}{stockToBuy}个，已被禁止", Game1.MasterPlayer.UniqueMultiplayerID);
            return false;
        }

        return true;
    }

    private static void TryToPurchaseItemPostfix(ISalable item, int stockToBuy)
    {
        MultiplayerLog.NoIconHUDMessage($"{Game1.player.Name}购买了 {stockToBuy} 个{item.DisplayName}", 500);
    }
}