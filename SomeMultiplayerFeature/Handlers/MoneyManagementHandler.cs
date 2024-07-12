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
    private const string MoneyManagementKey = "weizinai.SomeMultiplayerFeature_MoneyManagement";
    private const string DayMoneyLimitKey = "weizinai.SomeMultiplayerFeature_DayMoneyLimit";

    public MoneyManagementHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;

        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (!this.IsEnable()) return;

        this.SetDayMoneyLimit(this.Config.DayMoneyLimit.ToString());

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

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Game1.IsClient && this.CheckMoneyManagementEnable() && this.Config.ApplyForMoneyKey.JustPressed())
        {
            var targetMoney = Math.Min(Game1.MasterPlayer.Money, this.GetDayMoneyLimit());

            Game1.player.team.AddIndividualMoney(Game1.player, targetMoney);
            Game1.MasterPlayer.Money -= targetMoney;
            Log.Info($"成功申请转移金钱：{targetMoney}");
        }
    }

    private bool IsEnable()
    {
        return this.Config.MoneyManagement && Game1.IsServer;
    }

    private void SetDayMoneyLimit(string dayMoneyLimit)
    {
        var modData = Game1.player.modData;
        if (!modData.TryAdd(DayMoneyLimitKey, dayMoneyLimit))
            modData[DayMoneyLimitKey] = dayMoneyLimit;
    }

    private int GetDayMoneyLimit()
    {
        var modData = Game1.MasterPlayer.modData;
        return int.Parse(modData[DayMoneyLimitKey]);
    }

    private bool CheckMoneyManagementEnable()
    {
        var modData = Game1.MasterPlayer.modData;

        if (!modData.ContainsKey(MoneyManagementKey)) return false;

        return modData[MoneyManagementKey] == "true";
    }
}