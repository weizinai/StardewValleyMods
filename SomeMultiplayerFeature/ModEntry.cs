using Common.Patch;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Handlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature;

public class ModEntry : Mod
{
    private ModConfig config = null!;
    private AccessShopInfoHandler accessShopInfoHandler = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        accessShopInfoHandler = new AccessShopInfoHandler(helper, config);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this);
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        accessShopInfoHandler.OnModMessageReceived(e);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        accessShopInfoHandler.Update();
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