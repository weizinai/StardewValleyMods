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
    public const string SpendLimitKey = ModEntry.ModDataPrefix + "SpendLimit";
    public const string SpentLimitDataKey = ModEntry.ModDataPrefix + "SpentLimitData";
    public const string SpentAmountKey = ModEntry.ModDataPrefix + "SpentAmount";

    private static string LimitDataPath => $"data/purchase_limit_data/{Constants.SaveFolderName}.json";

    public SpendLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitSpendLimitConfig();
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;

        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;

        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
        this.Helper.Events.GameLoop.DayStarted -= this.OnDayStarted;

        this.Helper.Events.Multiplayer.PeerConnected -= this.OnPeerConnected;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.InitSpendLimitConfig();
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

        if (!this.Config.SpendLimit) return;

        var farmerName = Game1.getFarmer(e.Peer.PlayerID).Name;
        if (!SpendLimitHelper.TryGetFarmerSpendLimit(farmerName, out _))
        {
            SpendLimitHelper.SetFarmerSpendLimit(farmerName, this.Config.DefaultSpendLimit);
            Log.Info($"{farmerName}没有额度信息，已将其额度设置为{this.Config.DefaultSpendLimit}金");
        }
    }

    private void InitSpendLimitConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (this.Config.SpendLimit)
        {
            var rawData = this.Helper.Data.ReadJsonFile<Dictionary<string, int>>(LimitDataPath);
            var farmhands = Game1.getAllFarmhands()
                .Where(x => !x.isUnclaimedFarmhand)
                .Select(x => x.Name);

            if (rawData is null)
            {
                rawData = new Dictionary<string, int>();
                foreach (var name in farmhands) rawData[name] = this.Config.DefaultSpendLimit;
                Log.NoIconHUDMessage($"存档<{Constants.SaveFolderName}>没有额度信息，已自动将所有玩家的额度设置为{this.Config.DefaultSpendLimit}金");
            }
            else
            {
                foreach (var name in farmhands)
                {
                    if (!rawData.ContainsKey(name))
                    {
                        rawData[name] = this.Config.DefaultSpendLimit;
                        Log.NoIconHUDMessage($"{name}没有额度信息，已将其额度设置为{this.Config.DefaultSpendLimit}金");
                    }
                }
            }

            modData[SpendLimitKey] = "true";
            modData[SpentLimitDataKey] = JsonSerializer.Serialize(rawData);
        }
        else
        {
            modData.Remove(SpendLimitKey);
            Log.NoIconHUDMessage("花钱限制功能已关闭");
        }
    }
}