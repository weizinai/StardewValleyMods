using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature;

public class ModEntry : Mod
{
    public const string ModDataPrefix = "weizinai.SMF.";

    private GenericModConfigMenuIntegration<ModConfig>? configMenu;

    private IHandler[] handlers = Array.Empty<IHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Logger.Init(this.Monitor);
        Broadcaster.Init(this);
        ModConfig.Init(helper);
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // 注册GenericModConfigMenu
        this.configMenu = this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForSomeMultiplayerFeature(),
            () => ModConfig.Instance,
            value => ModConfig.Instance = value,
            this.UpdateConfig,
            this.UpdateConfig
        );

        // 注册Harmony补丁
        HarmonyPatcher.Apply(
            this.ModManifest.UniqueID,
            new FarmerPatcher(),
            new FarmHousePatcher(),
            new Game1Patcher(),
            new GameLocationPatcher()
            // new ShopMenuPatcher()
        );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModConfig.Instance.OpenConfigMenuKey.JustPressed())
        {
            this.configMenu?.OpenModMenu();
        }
    }

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers) handler.Clear();

        this.handlers = new IHandler[]
        {
            new AutoClickHandler(this.Helper),
            new CustomCommandHandler(this.Helper),
            new DataHandler(this.Helper),
            new IpConnectionHandler(this.Helper),
            new PerfectFishingHandler(this.Helper),
            new PlayerCountHandler(this.Helper),
            // new SpendLimitHandler(this.Helper, this.config),
            new TipHandler(this.Helper),
            new VersionLimitHandler(this.Helper)
        };

        foreach (var handler in this.handlers) handler.Apply();
    }
}