using System.Text.Json;
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
        this.Helper.ConsoleCommands.Add("reload_limit", "", this.ReloadLimit);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.ReadLimitData();
        this.InitPurchaseLimitConfig();
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
        if (Game1.IsClient) return;

        var farmerName = Game1.getFarmer(e.Peer.PlayerID).Name;
        if (!this.limitData.ContainsKey(farmerName))
        {
            this.limitData[farmerName] = this.Config.DefaultPurchaseLimit;
            this.SetPurchaseLimit();
            this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
            Log.Info($"{farmerName}玩家未设置额度，已自动将其设置为默认值{this.Config.DefaultPurchaseLimit}元");
        }
    }

    private void InitPurchaseLimitConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (this.Config.PurchaseLimit)
            modData[PurchaseLimitKey] = JsonSerializer.Serialize(this.limitData);
        else
            modData.Remove(PurchaseLimitKey);
    }

    private void ReadLimitData()
    {
        if (Game1.IsClient) return;

        Log.Info($"开始读取存档<{Constants.SaveFolderName}>的额度信息");

        var rawData = this.Helper.Data.ReadJsonFile<Dictionary<string, int>>(LimitDataPath);

        if (rawData is null)
        {
            foreach (var farmer in Game1.getAllFarmhands())
            {
                this.limitData[farmer.Name] = this.Config.DefaultPurchaseLimit;
            }
            this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
            Log.Info($"存档<{Constants.SaveFolderName}>没有额度信息，已自动创建，并将所有玩家的额度设置为{this.Config.DefaultPurchaseLimit}元");
            return;
        }

        this.limitData = rawData;

        Log.Info($"成功读取存档<{Constants.SaveFolderName}>的额度信息");
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
            Log.Info(money >= 0 ? $"已为所有玩家增加{money}元的额度" : $"为所有玩家减少{-money}元的额度");
        }
        else
        {
            var name = args[1];
            this.limitData[name] += money;
            Log.Info(money >= 0 ? $"已为{name}增加{money}元的额度" : $"已为{name}减少{-money}元的额度");
        }

        this.SetPurchaseLimit();
        this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
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

        this.SetPurchaseLimit();
        this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
    }

    private void ReloadLimit(string command, string[] args)
    {
        if (Game1.IsClient) return;

        this.ReadLimitData();
        this.SetPurchaseLimit();
    }

    private void SetPurchaseLimit()
    {
        var modData = Game1.MasterPlayer.modData;
        if (modData.ContainsKey(PurchaseLimitKey)) modData[PurchaseLimitKey] = JsonSerializer.Serialize(this.limitData);
    }
}