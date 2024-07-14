using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class PurchaseLimitHandler : BaseHandlerWithConfig<ModConfig>
{
    public const string PurchaseLimitKey = "weizinai.SomeMultiplayerFeature_PurchaseLimit";
    public const string PurchaseAmountKey = "weizinai.SomeMultiplayerFeature_PurchaseAmount";

    private static string LimitDataPath => $"data/purchase_limit_data/{Constants.SaveFolderName}.json";
    private Dictionary<string, int> limitData = new();

    public PurchaseLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitPurchaseLimitConfig();
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;

        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.GameLaunched -= this.OnGameLaunched;
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted -= this.OnDayStarted;

        this.Helper.Events.Multiplayer.PeerConnected -= this.OnPeerConnected;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.Helper.ConsoleCommands.Add("change_limit", "", this.ChangeLimit);
        this.Helper.ConsoleCommands.Add("set_limit", "", this.SetLimit);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.InitPurchaseLimitConfig();
        this.InitLimitData();
        this.InitFarmerLimitData();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (Game1.IsClient)
        {
            Game1.player.modData[PurchaseAmountKey] = "0";
            Log.NoIconHUDMessage("今日消费金额已重置");
        }
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (!this.Config.PurchaseLimit) return;

        if (Game1.IsClient) return;

        var farmer = Game1.getFarmer(e.Peer.PlayerID);
        if (!farmer.modData.ContainsKey(PurchaseLimitKey))
        {
            farmer.modData[PurchaseLimitKey] = this.Config.DefaultPurchaseLimit.ToString();
            this.limitData[farmer.Name] = this.Config.DefaultPurchaseLimit;
            this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
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

    private void ChangeLimit(string command, string[] args)
    {
        if (Game1.IsClient) return;

        if (args.Length == 0 || args.Length > 2 || !int.TryParse(args[0], out var money))
        {
            Log.Error("命令输入错误，请使用：change_limit <金额> [玩家名称]");
            return;
        }

        if (args.Length == 1)
        {
            foreach (var name in this.limitData.Keys)
            {
                this.limitData[name] += money;
            }
            Log.Info(money >= 0 ? $"已为所有玩家增加{money}元的购物额度" : $"为所有玩家减少{-money}元的购物额度");
        }
        else
        {
            var name = args[1];
            this.limitData[name] += money;
            Log.Info(money >= 0 ? $"已为{name}增加{money}元的购物额度" : $"已为{name}减少{-money}元的购物额度");
        }

        this.InitFarmerLimitData();
    }

    private void SetLimit(string command, string[] args)
    {
        if (Game1.IsClient) return;

        if (args.Length == 0 || args.Length > 2 || !int.TryParse(args[0], out var money))
        {
            Log.Error("命令输入错误，请使用：set_limit <金额> [玩家名称]");
            return;
        }

        if (args.Length == 1)
        {
            foreach (var name in this.limitData.Keys)
            {
                this.limitData[name] = money;
            }
            Log.Info($"已将所有玩家的购物额度设置为{money}元");
        }
        else
        {
            var name = args[1];
            this.limitData[name] = money;
            Log.Info($"已将{name}的购物额度设置为{money}元");
        }

        this.InitFarmerLimitData();
    }
}