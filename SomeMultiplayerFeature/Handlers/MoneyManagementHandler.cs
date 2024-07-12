using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class MoneyManagementHandler : BaseHandlerWithConfig<ModConfig>
{
    public MoneyManagementHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (!this.IsEnable()) return;

        if (!Game1.player.useSeparateWallets)
        {
            ManorHouse.SeparateWallets();
            Log.Info("当前存档不是独立资金，已自动转化为独立资金");
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!this.IsEnable()) return;

        Log.Alert("开始转移金钱");

        foreach (var farmer in Game1.getAllFarmhands().Where(x => !x.isUnclaimedFarmhand))
        {
            var money = farmer.team.GetIndividualMoney(farmer);

            Log.Info($"成功转移{farmer.Name}的金钱：{money}");
            farmer.team.AddIndividualMoney(Game1.MasterPlayer, money);
            farmer.team.SetIndividualMoney(farmer, 0);
        }
    }

    private bool IsEnable()
    {
        return this.Config.MoneyManagement && Game1.IsServer;
    }
}