using Common;
using LazyMod.Framework;
using LazyMod.Framework.Config;
using LazyMod.Framework.Hud;
using LazyMod.Framework.Integration;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LazyMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private AutomationManger automationManger = null!;
    private MiningHud miningHud = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(Monitor);
        I18n.Init(helper.Translation);
        try
        {
            config = helper.ReadConfig<ModConfig>();
        }
        catch (Exception)
        {
            helper.WriteConfig(new ModConfig());
            config = helper.ReadConfig<ModConfig>();
            Log.Info("Read config.json file failed and was automatically fixed. Please reset the features you want to turn on.");
        }
        automationManger = new AutomationManger(helper, config);

        // 注册事件
        helper.Events.Display.RenderedHud += OnRenderedHud;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
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
        
        new GenericModConfigMenuIntegrationForLazyMod(
            Helper,
            ModManifest,
            () => config,
            () =>
            {
                config = new ModConfig();
                UpdateConfig();
            },
            () => Helper.WriteConfig(config)
        ).Register();
    }

    private void UpdateConfig()
    {
        automationManger.UpdateConfig(config);
    }
}