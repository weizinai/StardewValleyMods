﻿using Common.Integrations;
using Common.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using TestMod.Framework;

namespace TestMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private IClickableMenu? lastMenu;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.Key.JustPressed())
            Game1.activeClickableMenu = new ShippingMenu(new List<Item>());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        var test = new TestWheelSpin(10000);
        
        test.Update();
        
        Monitor.Log($"{test.count/(double)test.total}", LogLevel.Info);

        // var count = 0;
        //
        // for (int i = 0; i < 100000; i++)
        // {
        //     var randomInt = Game1.random.Next(0, 15);
        //     var randomBool = Game1.random.NextBool();
        //     if (randomInt == 1 && randomBool == false) continue;
        //     if (randomInt == 4 && randomBool == false) continue;
        //     if (randomInt == 5 && randomBool == false) continue;
        //     if (randomInt == 0 && randomBool) continue;
        //     if (randomInt == 1 && randomBool) continue;
        //     if (randomInt == 11 && randomBool) continue;
        //     if (randomInt == 12 && randomBool) continue;
        //     if (randomInt == 13 && randomBool) continue;
        //     count++;
        // }
        //
        // Monitor.Log($"{count / 100000.0}", LogLevel.Info);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // if (Game1.activeClickableMenu is WheelSpinGame menu && lastMenu is not WheelSpinGame)
        // {
        //     Monitor.Log($"初始随机速度: {menu.arrowRotationVelocity}", LogLevel.Info);
        //
        //     menu.arrowRotationVelocity = Math.PI / 16.0;
        //     menu.arrowRotationVelocity += config.RandomInt * Math.PI / 256.0;
        //     if (config.RandomBool)
        //     {
        //         menu.arrowRotationVelocity += Math.PI / 64.0;
        //     }
        //
        //     Monitor.Log($"修改后随机速度({config.RandomInt}-{config.RandomBool}): {menu.arrowRotationVelocity}", LogLevel.Info);
        // }

        if (Game1.activeClickableMenu is WheelSpinGame gameMenu)
        {
            for (int i = 0; i < 10; i++)
            {
                gameMenu.update(Game1.currentGameTime);
            }
        }

        // lastMenu = Game1.activeClickableMenu;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => config.RandomInt,
            value => config.RandomInt = value,
            I18n.Config_RandomInt_Name,
            null,
            0,
            14
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.RandomBool,
            value => config.RandomBool = value,
            I18n.Config_RandomBool_Name
        );

        // configMenu.AddNumberOption(
        //     ModManifest,
        //     () => config.MineShaftMap,
        //     value => config.MineShaftMap = value,
        //     I18n.Config_MineShaftMap_Name,
        //     I18n.Config_MineShaftMap_ToolTip,
        //     40,
        //     60
        // );
        //
        // configMenu.AddNumberOption(
        //     ModManifest,
        //     () => config.VolcanoDungeonMap,
        //     value => config.VolcanoDungeonMap = value,
        //     I18n.Config_VolcanoDungeonMap_Name,
        //     I18n.Config_VolcanoDungeonMap_ToolTip,
        //     38,
        //     57
        // );
    }
}