using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature.Handlers;

internal class MoneyCheatHandler : BaseHandler
{
    private int lastMoney;

    public MoneyCheatHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        Helper.Events.GameLoop.DayStarted += OnDayStarted;
        Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    public void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (!Config.BanMoneyCheat) return;

        lastMoney = Game1.player.Money * 10;
    }

    public void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!Config.BanMoneyCheat) return;

        lastMoney = Game1.player.Money * 10;
    }

    public void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Config.BanMoneyCheat || !Context.IsWorldReady) return;

        if (Game1.activeClickableMenu is ShippingMenu) return;

        if (Game1.player.Money - lastMoney / 10 >= 100000)
        {
            Log.Alert("检测到疑似有人作弊");
            Game1.player.Money = lastMoney / 10;
        }

        lastMoney = Game1.player.Money * 10;
    }
}