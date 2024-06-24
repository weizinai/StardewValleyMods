using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

namespace weizinai.StardewValleyMod.MultiplayerModLimit;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.IsClient)
        {
            
        }
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        // 如果玩家不是多人模式的房主，则返回
        if (!Game1.IsServer) return;
        
        
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForMultiplayerModLimit(
            this.Helper,
            this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }
}