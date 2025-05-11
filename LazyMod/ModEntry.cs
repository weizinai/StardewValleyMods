﻿using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Handler;
using weizinai.StardewValleyMod.LazyMod.Helper;
using weizinai.StardewValleyMod.LazyMod.Integration;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.LazyMod;

public class ModEntry : Mod
{
    private ModConfig config = null!;
    private GenericModConfigMenuIntegration<ModConfig>? configMenu;

    private int cooldownTimer;
    private bool modEnable = true;

    private IAutomationHandler[] handlers = null!;
    private IAutomationHandlerWithDayChanged[] dayChangedHandlers = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger.Init(this.Monitor);
        try
        {
            this.config = helper.ReadConfig<ModConfig>();
        }
        catch (Exception)
        {
            helper.WriteConfig(new ModConfig());
            this.config = helper.ReadConfig<ModConfig>();
            Logger.Info("Read config.json file failed and was automatically fixed. Please reset the features you want to turn on.");
        }

        this.UpdateConfig();

        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.DayEnding += this.OnDayEnding;

        helper.Events.Player.InventoryChanged += this.OnInventoryChanged;

        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.configMenu = this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForLazyMod(),
            () => this.config,
            value => this.config = value,
            this.UpdateConfig,
            this.UpdateConfig
        );
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        ToolHelper.UpdateToolCache();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        foreach (var handler in this.dayChangedHandlers) handler.OnDayStarted();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!this.modEnable) return;

        if (!this.UpdateCooldown()) return;

        var player = Game1.player;
        var location = Game1.currentLocation;
        if (player is null || location is null) return;

        var item = player.CurrentItem;

        TileHelper.ClearTileCache();
        foreach (var handler in this.handlers)
        {
            if (handler.IsEnable())
            {
                handler.Apply(item, player, location);
            }
        }
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        foreach (var handler in this.dayChangedHandlers) handler.OnDayEnding();
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (e.Added.Any(item => item is Tool) || e.Removed.Any(item => item is Tool))
            ToolHelper.UpdateToolCache();
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.config.ToggleModStateKeybind.JustPressed())
        {
            this.modEnable = !this.modEnable;
            Logger.NoIconHUDMessage(this.modEnable ? I18n.UI_ModState_Enable() : I18n.UI_ModState_Disable());
        }

        if (this.config.OpenConfigMenuKeybind.JustPressed())
        {
            this.configMenu?.OpenModMenu();
        }
    }

    private bool UpdateCooldown()
    {
        this.cooldownTimer++;
        if (this.cooldownTimer < this.config.Cooldown) return false;
        this.cooldownTimer = 0;
        return true;
    }

    private void UpdateConfig()
    {
        this.handlers = this.GetHandlers().ToArray();
        this.dayChangedHandlers = this.handlers.OfType<IAutomationHandlerWithDayChanged>().ToArray();
    }

    private IEnumerable<IAutomationHandler> GetHandlers()
    {
        // Farming
        if (this.config.AutoTillDirt.IsEnable) yield return new TillDirtHandler(this.config);
        if (this.config.AutoClearTilledDirt.IsEnable) yield return new ClearTilledDirtHandler(this.config);
        if (this.config.AutoWaterDirt.IsEnable) yield return new WaterDirtHandler(this.config);
        if (this.config.AutoRefillWateringCan.IsEnable) yield return new RefillWateringCanHandler(this.config);
        if (this.config.AutoSeed.IsEnable) yield return new SeedHandler(this.config);
        if (this.config.AutoFertilize.IsEnable) yield return new FertilizeHandler(this.config);
        if (this.config.AutoHarvestCrop.IsEnable) yield return new HarvestCropHandler(this.config);
        if (this.config.AutoShakeFruitTree.IsEnable) yield return new ShakeFruitTreeHandler(this.config);
        if (this.config.AutoClearDeadCrop.IsEnable) yield return new ClearDeadCropHandler(this.config);

        // Animal
        if (this.config.AutoPetAnimal.IsEnable) yield return new PetAnimalHandler(this.config);
        if (this.config.AutoPetPet.IsEnable) yield return new PetPetHandler(this.config);
        if (this.config.AutoMilkAnimal.IsEnable) yield return new MilkAnimalHandler(this.config);
        if (this.config.AutoShearsAnimal.IsEnable) yield return new ShearsAnimalHandler(this.config);
        if (this.config.AutoFeedAnimalCracker.IsEnable) yield return new AnimalCrackerHandler(this.config);
        if (this.config.AutoOpenAnimalDoor) yield return new AnimalDoorHandler(this.config);
        if (this.config.AutoOpenFenceGate.IsEnable) yield return new FenceGateHandler(this.config);

        // Mining
        if (this.config.AutoClearStone.IsEnable) yield return new ClearStoneHandler(this.config);
        if (this.config.AutoCollectCoal.IsEnable) yield return new CollectCoalHandler(this.config);
        if (this.config.AutoBreakContainer.IsEnable) yield return new BreakContainerHandler(this.config);
        if (this.config.AutoOpenTreasure.IsEnable) yield return new OpenTreasureHandler(this.config);
        if (this.config.AutoClearCrystal.IsEnable) yield return new ClearCrystalHandler(this.config);
        if (this.config.AutoCoolLava.IsEnable) yield return new CoolLavaHandler(this.config);

        // Foraging
        if (this.config.AutoForage.IsEnable) yield return new ForageHandler(this.config);
        if (this.config.AutoHarvestGinger.IsEnable) yield return new HarvestGingerHandler(this.config);
        if (this.config.AutoChopTree.IsEnable) yield return new ChopTreeHandler(this.config);
        if (this.config.AutoShakeTree.IsEnable) yield return new ShakeTreeHandler(this.config);
        if (this.config.AutoHarvestMoss.IsEnable) yield return new HarvestMossHandler(this.config);
        if (this.config.AutoPlaceTapper.IsEnable) yield return new PlaceTapperHandler(this.config);
        if (this.config.AutoPlaceVinegar.IsEnable) yield return new PlaceVinegarHandler(this.config);
        if (this.config.AutoClearWood.IsEnable) yield return new ClearWoodHandler(this.config);

        // Fishing
        if (this.config.AutoGrabTreasureItem) yield return new GrabTreasureItemHandler(this.config);
        if (this.config.AutoExitTreasureMenu) yield return new ExitTreasureMenuHandler(this.config);
        if (this.config.AutoPlaceCarbPot.IsEnable) yield return new PlaceCrabPotHandler(this.config);
        if (this.config.AutoAddBaitForCarbPot.IsEnable) yield return new AddBaitForCrabPotHandler(this.config);
        if (this.config.AutoHarvestCarbPot.IsEnable) yield return new HarvestCrabPotHandler(this.config);

        // Food
        yield return new FoodHandler(this.config);

        // Other
        yield return new MagneticRadiusHandler(this.config);
        if (this.config.AutoClearWeeds.IsEnable) yield return new ClearWeedsHandler(this.config);
        if (this.config.AutoDigSpots.IsEnable) yield return new DigSpotHandler(this.config);
        if (this.config.AutoHarvestMachine.IsEnable) yield return new HarvestMachineHandler(this.config);
        if (this.config.AutoTriggerMachine.IsEnable) yield return new TriggerMachineHandler(this.config);
        if (this.config.AutoUseFairyDust.IsEnable) yield return new FairyDustHandler(this.config);
        if (this.config.AutoGarbageCan.IsEnable) yield return new GarbageCanHandler(this.config);
        if (this.config.AutoPlaceFloor.IsEnable) yield return new PlaceFloorHandler(this.config);
    }
}