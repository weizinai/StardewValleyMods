using LazyMod.Framework;
using LazyMod.Framework.Hud;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LazyMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private AutomationManger automationManger = null!;
    private MiningHud miningHud = null!;
    private GenericModConfigMenuIntegrationForLazyMod integration = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        automationManger = new AutomationManger(helper, config);
        integration = new GenericModConfigMenuIntegrationForLazyMod(
            Helper.ModRegistry,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );
        I18n.Init(helper.Translation);

        // 注册事件
        helper.Events.Display.RenderedHud += OnRenderedHud;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;

        // 迁移
        Migrate();
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.OpenConfigMenuKeybind.JustPressed()) integration.OpenMenu();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        miningHud.Update();
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        miningHud.Draw(e.SpriteBatch);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        miningHud = new MiningHud(Helper, config);

        integration.Register();
    }

    private void Migrate()
    {
        // 1.0.8
        config.ChopOakTree.TryAdd(3, false);
        config.ChopMapleTree.TryAdd(3, false);
        config.ChopPineTree.TryAdd(3, false);
        config.ChopMahoganyTree.TryAdd(3, false);
        config.ChopPalmTree.TryAdd(3, false);
        config.ChopMushroomTree.TryAdd(3, false);
        config.ChopGreenRainTree.TryAdd(3, false);
        config.ChopMysticTree.TryAdd(3, false);
    }
}