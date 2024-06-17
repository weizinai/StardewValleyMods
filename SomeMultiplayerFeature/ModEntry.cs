using Common;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Handlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SomeMultiplayerFeature;

public class ModEntry : Mod
{
    private ModConfig config = null!;
    private AccessShopInfoHandler accessShopInfoHandler = null!;
    private DelayedPlayerHandler delayedPlayerHandler = null!;
    private ModLimitHandler modLimitHandler = null!;
    private PlayerCountHandler playerCountHandler = null!;
    private UnreadyPlayerHandler unreadyPlayerHandler = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Log.Init(Monitor);
        config = helper.ReadConfig<ModConfig>();
        accessShopInfoHandler = new AccessShopInfoHandler(helper, config);
        delayedPlayerHandler = new DelayedPlayerHandler(config);
        modLimitHandler = new ModLimitHandler(helper, config);
        playerCountHandler = new PlayerCountHandler(config);
        unreadyPlayerHandler = new UnreadyPlayerHandler(config);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.OneSecondUpdateTicked += OnSecondUpdateTicked;
        helper.Events.Display.Rendered += OnRendered;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
        helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
        helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        accessShopInfoHandler.OnUpdateTicked();
    }

    private void OnSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        delayedPlayerHandler.OnSecondUpdateTicked();
        playerCountHandler.OnSecondUpdateTicked();
    }

    private void OnRendered(object? sender, RenderedEventArgs e)
    {
        playerCountHandler.OnRendered(e.SpriteBatch);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        unreadyPlayerHandler.OnButtonChanged();
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        modLimitHandler.OnPeerConnected(e);
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        accessShopInfoHandler.OnModMessageReceived(e);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForSomeMultiplayerFeature(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }
}