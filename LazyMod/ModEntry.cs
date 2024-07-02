﻿using weizinai.StardewValleyMod.Common.Log;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Hud;
using weizinai.StardewValleyMod.LazyMod.Framework.Integration;
using weizinai.StardewValleyMod.LazyMod.Handler;
using weizinai.StardewValleyMod.LazyMod.Handler.Farming;

namespace weizinai.StardewValleyMod.LazyMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private MiningHud miningHud = null!;
    private IAutomationHandler[] handlers = null!;
    private IAutomationHandlerWithDayChanged[] dayChangedHandlers = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        try
        {
            this.config = helper.ReadConfig<ModConfig>();
        }
        catch (Exception)
        {
            helper.WriteConfig(new ModConfig());
            this.config = helper.ReadConfig<ModConfig>();
            Log.Info("Read config.json file failed and was automatically fixed. Please reset the features you want to turn on.");
        }

        this.UpdateConfig();

        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.DayEnding += this.OnDayEnding;
        helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        helper.Events.Display.RenderedHud += this.OnRenderedHud;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        ToolHelper.UpdateToolCache();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        foreach (var handler in this.dayChangedHandlers)
        {
            handler.OnDayStarted();
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        var player = Game1.player;
        var location = Game1.currentLocation;
        if (player is null || location is null) return;
        
        TileHelper.ClearTileCache();
        foreach (var handler in this.handlers) handler.Apply(player, location);

        this.miningHud.Update();
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        foreach (var handler in this.dayChangedHandlers)
        {
            handler.OnDayEnding();
        }
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        ToolHelper.UpdateToolCache();
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        this.miningHud.Draw(e.SpriteBatch);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.miningHud = new MiningHud(this.Helper, this.config);

        new GenericModConfigMenuIntegrationForLazyMod(
            this.Helper,
            this.ModManifest,
            () => this.config,
            () =>
            {
                this.config = new ModConfig();
                this.Helper.WriteConfig(this.config);
                this.UpdateConfig();
            },
            () =>
            {
                this.Helper.WriteConfig(this.config);
                this.UpdateConfig();
            }
        ).Register();
    }

    private void UpdateConfig()
    {
        this.handlers = this.GetHandlers(this.config).ToArray();
        this.dayChangedHandlers = this.handlers.OfType<IAutomationHandlerWithDayChanged>().ToArray();
    }

    private IEnumerable<IAutomationHandler> GetHandlers(ModConfig _config)
    {
        // Farming
        if (_config.AutoTillDirt.IsEnable) yield return new TillDirtHandler(_config);
        if (_config.AutoClearTilledDirt.IsEnable) yield return new ClearTilledDirtHandler(_config);
        if (_config.AutoWaterDirt.IsEnable) yield return new WaterDirtHandler(_config);
        if (_config.AutoRefillWateringCan.IsEnable) yield return new RefillWateringCanHandler(_config);
        if (_config.AutoSeed.IsEnable) yield return new SeedHandler(_config);
        if (_config.AutoFertilize.IsEnable) yield return new FertilizeHandler(_config);
        if (_config.AutoHarvestCrop.IsEnable) yield return new HarvestCropHandler(_config);
        if (_config.AutoShakeFruitTree.IsEnable) yield return new ShakeFruitTreeHandler(_config);
        if (_config.AutoClearDeadCrop.IsEnable) yield return new ClearDeadCropHandler(_config);
        
        // Animal
        if (_config.AutoPetAnimal.IsEnable) yield return new PetAnimalHandler(_config);
        if (_config.AutoPetPet.IsEnable) yield return new PetPetHandler(_config);
        if (_config.AutoMilkAnimal.IsEnable) yield return new MilkAnimalHandler(_config);
        if (_config.AutoShearsAnimal.IsEnable) yield return new ShearsAnimalHandler(_config);
        if (_config.AutoFeedAnimalCracker.IsEnable) yield return new AnimalCrackerHandler(_config);
        if (_config.AutoOpenFenceGate.IsEnable) yield return new FenceGateHandler(_config);
    }
}