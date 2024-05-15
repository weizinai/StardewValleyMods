﻿using LazyMod.Framework;
using LazyMod.Framework.Hud;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LazyMod;

public class ModEntry : Mod
{
    private ModConfig config = null!;
    private AutomationManger automationManger = null!;
    private MiningHud miningHud = null!;

    public override void Entry(IModHelper helper)
    {
        // 读取配置文件
        config = helper.ReadConfig<ModConfig>();
        Migrate();

        // 初始化
        I18n.Init(helper.Translation);
        automationManger = new AutomationManger(helper, config);

        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Display.RenderedHud += OnRenderedHud;
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
            Helper.ModRegistry,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
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