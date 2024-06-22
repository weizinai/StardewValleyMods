using weizinai.StardewValleyMod.Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Hud;
using weizinai.StardewValleyMod.LazyMod.Framework.Integration;

namespace weizinai.StardewValleyMod.LazyMod;

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