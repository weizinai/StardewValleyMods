using System.Text.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class SpendLimitHandler : BaseHandlerWithConfig<ModConfig>
{
    public const string SpentLimitKey = ModEntry.ModDataPrefix + "SpentLimit";
    public const string SpentAmountKey = ModEntry.ModDataPrefix + "SpentAmount";

    private static string LimitDataPath => $"data/purchase_limit_data/{Constants.SaveFolderName}.json";
    private Dictionary<string, int> limitData = new();

    public SpendLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitPurchaseLimitConfig();
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;

        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;

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
        this.Helper.ConsoleCommands.Add("set_limit", "", this.SetLimit);
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
            Game1.player.modData[SpentAmountKey] = "0";
            Log.NoIconHUDMessage("今日消费金额已重置");
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.Config.SpendLimitManagerMenuKey.JustPressed())
        {
            if (Game1.IsServer)
                Game1.activeClickableMenu = new SpendLimitManagerMenu();
            else
                Log.Error("客户端无法打开花钱限制管理菜单，后续按该按键会显示额度相关信息");
        }
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (Game1.IsClient) return;

        var farmerName = Game1.getFarmer(e.Peer.PlayerID).Name;
        if (!this.limitData.ContainsKey(farmerName))
        {
            this.limitData[farmerName] = this.Config.DefaultSpendLimit;
            this.SetPurchaseLimit();
            this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
            Log.Info($"{farmerName}玩家未设置额度，已自动将其设置为默认值{this.Config.DefaultSpendLimit}元");
        }
    }

    private void InitPurchaseLimitConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (this.Config.SpendLimit)
            modData[SpentLimitKey] = JsonSerializer.Serialize(this.limitData);
        else
            modData.Remove(SpentLimitKey);
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
                this.limitData[farmer.Name] = this.Config.DefaultSpendLimit;
            }
            this.Helper.Data.WriteJsonFile(LimitDataPath, this.limitData);
            Log.Info($"存档<{Constants.SaveFolderName}>没有额度信息，已自动创建，并将所有玩家的额度设置为{this.Config.DefaultSpendLimit}元");
            return;
        }

        this.limitData = rawData;

        Log.Info($"成功读取存档<{Constants.SaveFolderName}>的额度信息");
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

    private void SetPurchaseLimit()
    {
        var modData = Game1.MasterPlayer.modData;
        if (modData.ContainsKey(SpentLimitKey)) modData[SpentLimitKey] = JsonSerializer.Serialize(this.limitData);
    }
}