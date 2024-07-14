using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class PurchaseItemLimitHandler : BaseHandlerWithConfig<ModConfig>
{
    public const string PurchaseLimitKey = "weizinai.SomeMultiplayerFeature_PurchaseLimit";
    public const string PurchaseAmountKey = "weizinai.SomeMultiplayerFeature_PurchaseAmount";

    private static string LimitDataPath => $"data/purchase_limit_data/{Constants.SaveFolderName}.json";
    private Dictionary<string, int> limitData = new();

    public PurchaseItemLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitPurchaseLimitConfig();
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted -= this.OnDayStarted;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.InitPurchaseLimitConfig();
        this.InitLimitData();
        this.InitFarmerLimitData();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (Game1.IsClient) return;

        Log.Alert("重置今日消费金额");
        foreach (var farmer in Game1.getAllFarmhands())
        {
            farmer.modData[PurchaseAmountKey] = "0";
        }
    }

    private void InitPurchaseLimitConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (this.Config.PurchaseLimit)
            modData[PurchaseLimitKey] = "true";
        else
            modData.Remove(PurchaseLimitKey);
    }

    private void InitLimitData()
    {
        if (Game1.IsClient) return;

        var rawData = this.Helper.Data.ReadJsonFile<Dictionary<string, int>>(LimitDataPath);

        if (rawData is null)
        {
            foreach (var farmer in Game1.getAllFarmhands())
            {
                this.limitData[farmer.Name] = this.Config.DefaultPurchaseLimit;
            }
            this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
            return;
        }

        this.limitData = rawData;
    }

    private void InitFarmerLimitData()
    {
        if (Game1.IsClient) return;

        foreach (var farmer in Game1.getAllFarmhands())
        {
            if (this.limitData.TryGetValue(farmer.Name, out var value))
            {
                farmer.modData[PurchaseLimitKey] = value.ToString();
            }
            else
            {
                farmer.modData[PurchaseLimitKey] = this.Config.DefaultPurchaseLimit.ToString();
                this.limitData[farmer.Name] = this.Config.DefaultPurchaseLimit;
            }
        }

        this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
    }
}