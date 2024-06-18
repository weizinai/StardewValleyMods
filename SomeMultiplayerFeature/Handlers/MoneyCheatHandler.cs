using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature.Handlers;

internal class MoneyCheatHandler
{
    private readonly ModConfig config;

    private int lastMoney;

    public MoneyCheatHandler(ModConfig config)
    {
        this.config = config;
    }

    public void OnSaveLoaded()
    {
        if (!config.BanMoneyCheat) return;

        lastMoney = Game1.player.Money * 10;
    }

    public void OnDayStarted()
    {
        if (!config.BanMoneyCheat) return;

        lastMoney = Game1.player.Money * 10;
    }

    public void OnUpdateTicked()
    {
        if (!config.BanMoneyCheat || !Context.IsWorldReady) return;
        
        if (Game1.activeClickableMenu is ShippingMenu) return;

        if (Game1.player.Money - lastMoney / 10 >= 100000)
        {
            Log.Alert("检测到疑似有人作弊");
            Game1.player.Money = lastMoney/10;
        }
        
        lastMoney = Game1.player.Money * 10;
    }
}