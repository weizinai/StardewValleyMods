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
    private ModConfig Config => ModConfig.Instance;
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
        ModConfig.Init(helper);

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
            () => ModConfig.Instance,
            value => ModConfig.Instance = value,
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
        foreach (var handler in this.dayChangedHandlers)
        {
            handler.OnDayEnding();
        }
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (e.Added.Any(item => item is Tool) || e.Removed.Any(item => item is Tool))
        {
            ToolHelper.UpdateToolCache();
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.Config.ToggleModStateKeybind.JustPressed())
        {
            this.modEnable = !this.modEnable;
            Logger.NoIconHUDMessage(this.modEnable ? I18n.UI_ModState_Enable() : I18n.UI_ModState_Disable());
        }

        if (this.Config.OpenConfigMenuKeybind.JustPressed())
        {
            this.configMenu?.OpenModMenu();
        }
    }

    private bool UpdateCooldown()
    {
        this.cooldownTimer++;
        if (this.cooldownTimer < ModConfig.Instance.Cooldown) return false;
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
        if (this.Config.AutoTillDirt.IsEnable) yield return new TillDirtHandler();
        if (this.Config.AutoClearTilledDirt.IsEnable) yield return new ClearTilledDirtHandler();
        if (this.Config.AutoWaterDirt.IsEnable) yield return new WaterDirtHandler();
        if (this.Config.AutoRefillWateringCan.IsEnable) yield return new RefillWateringCanHandler();
        if (this.Config.AutoSeed.IsEnable) yield return new SeedHandler();
        if (this.Config.AutoFertilize.IsEnable) yield return new FertilizeHandler();
        if (this.Config.AutoHarvestCrop.IsEnable) yield return new HarvestCropHandler();
        if (this.Config.AutoShakeFruitTree.IsEnable) yield return new ShakeFruitTreeHandler();
        if (this.Config.AutoClearDeadCrop.IsEnable) yield return new ClearDeadCropHandler();

        // Animal
        if (this.Config.AutoPetAnimal.IsEnable) yield return new PetAnimalHandler();
        if (this.Config.AutoPetPet.IsEnable) yield return new PetPetHandler();
        if (this.Config.AutoMilkAnimal.IsEnable) yield return new MilkAnimalHandler();
        if (this.Config.AutoShearsAnimal.IsEnable) yield return new ShearsAnimalHandler();
        if (this.Config.AutoFeedAnimalCracker.IsEnable) yield return new AnimalCrackerHandler();
        if (this.Config.AutoOpenAnimalDoor) yield return new AnimalDoorHandler();
        if (this.Config.AutoOpenFenceGate.IsEnable) yield return new FenceGateHandler();

        // Mining
        if (this.Config.AutoClearStone.IsEnable) yield return new ClearStoneHandler();
        if (this.Config.AutoCollectCoal.IsEnable) yield return new CollectCoalHandler();
        if (this.Config.AutoBreakContainer.IsEnable) yield return new BreakContainerHandler();
        if (this.Config.AutoOpenTreasure.IsEnable) yield return new OpenTreasureHandler();
        if (this.Config.AutoClearCrystal.IsEnable) yield return new ClearCrystalHandler();
        if (this.Config.AutoCoolLava.IsEnable) yield return new CoolLavaHandler();

        // Foraging
        if (this.Config.AutoForage.IsEnable) yield return new ForageHandler();
        if (this.Config.AutoHarvestGinger.IsEnable) yield return new HarvestGingerHandler();
        if (this.Config.AutoChopTree.IsEnable) yield return new ChopTreeHandler();
        if (this.Config.AutoShakeTree.IsEnable) yield return new ShakeTreeHandler();
        if (this.Config.AutoHarvestMoss.IsEnable) yield return new HarvestMossHandler();
        if (this.Config.AutoPlaceTapper.IsEnable) yield return new PlaceTapperHandler();
        if (this.Config.AutoPlaceVinegar.IsEnable) yield return new PlaceVinegarHandler();
        if (this.Config.AutoClearWood.IsEnable) yield return new ClearWoodHandler();

        // Fishing
        if (this.Config.AutoGrabTreasureItem) yield return new GrabTreasureItemHandler();
        if (this.Config.AutoExitTreasureMenu) yield return new ExitTreasureMenuHandler();
        if (this.Config.AutoPlaceCarbPot.IsEnable) yield return new PlaceCrabPotHandler();
        if (this.Config.AutoAddBaitForCarbPot.IsEnable) yield return new AddBaitForCrabPotHandler();
        if (this.Config.AutoHarvestCarbPot.IsEnable) yield return new HarvestCrabPotHandler();

        // Food
        yield return new FoodHandler();

        // Other
        yield return new MagneticRadiusHandler();
        if (this.Config.AutoClearWeeds.IsEnable) yield return new ClearWeedsHandler();
        if (this.Config.AutoDigSpots.IsEnable) yield return new DigSpotHandler();
        if (this.Config.AutoHarvestMachine.IsEnable) yield return new HarvestMachineHandler();
        if (this.Config.AutoTriggerMachine.IsEnable) yield return new TriggerMachineHandler();
        if (this.Config.AutoUseFairyDust.IsEnable) yield return new FairyDustHandler();
        if (this.Config.AutoGarbageCan.IsEnable) yield return new GarbageCanHandler();
        if (this.Config.AutoPlaceFloor.IsEnable) yield return new PlaceFloorHandler();
    }
}