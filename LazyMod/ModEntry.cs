using weizinai.StardewValleyMod.Common.Log;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;
using weizinai.StardewValleyMod.LazyMod.Framework.Integration;
using weizinai.StardewValleyMod.LazyMod.Handler;

namespace weizinai.StardewValleyMod.LazyMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
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
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        ToolHelper.UpdateToolCache();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        foreach (var handler in this.dayChangedHandlers)
        {
            handler.OnDayStarted();
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsPlayerFree && Game1.activeClickableMenu is not ItemGrabMenu { source: ItemGrabMenu.source_fishingChest }) return;

        var player = Game1.player;
        var location = Game1.currentLocation;
        if (player is null || location is null) return;

        var item = player.CurrentItem;
        if (item is null) return;

        TileHelper.ClearTileCache();
        foreach (var handler in this.handlers) handler.Apply(item, player, location);
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
        ToolHelper.UpdateToolCache();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
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
        if (_config.AutoOpenAnimalDoor) yield return new AnimalDoorHandler(_config);
        if (_config.AutoOpenFenceGate.IsEnable) yield return new FenceGateHandler(_config);

        // Mining
        if (_config.AutoClearStone.IsEnable) yield return new ClearStoneHandler(_config);
        if (_config.AutoCollectCoal.IsEnable) yield return new CollectCoalHandler(_config);
        if (_config.AutoBreakContainer.IsEnable) yield return new BreakContainerHandler(_config);
        if (_config.AutoOpenTreasure.IsEnable) yield return new OpenTreasureHandler(_config);
        if (_config.AutoClearCrystal.IsEnable) yield return new ClearCrystalHandler(_config);
        if (_config.AutoCoolLava.IsEnable) yield return new CoolLavaHandler(_config);

        // Foraging
        if (_config.AutoForage.IsEnable) yield return new ForageHandler(_config);
        if (_config.AutoHarvestGinger.IsEnable) yield return new HarvestGingerHandler(_config);
        if (_config.AutoChopTree.IsEnable) yield return new ChopTreeHandler(_config);
        if (_config.AutoShakeTree.IsEnable) yield return new ShakeTreeHandler(_config);
        if (_config.AutoHarvestMoss.IsEnable) yield return new HarvestCropHandler(_config);
        if (_config.AutoPlaceTapper.IsEnable) yield return new PlaceTapperHandler(_config);
        if (_config.AutoPlaceVinegar.IsEnable) yield return new PlaceVinegarHandler(_config);
        if (_config.AutoClearWood.IsEnable) yield return new ClearWoodHandler(_config);

        // Fishing
        if (_config.AutoGrabTreasureItem) yield return new GrabTreasureItemHandler(_config);
        if (_config.AutoExitTreasureMenu) yield return new ExitTreasureMenuHandler(_config);
        if (_config.AutoPlaceCarbPot.IsEnable) yield return new PlaceCrabPotHandler(_config);
        if (_config.AutoAddBaitForCarbPot.IsEnable) yield return new AddBaitForCrabPotHandler(_config);
        if (_config.AutoHarvestCarbPot.IsEnable) yield return new HarvestCrabPotHandler(_config);

        // Food
        yield return new FoodHandler(_config);

        // Other
        yield return new MagneticRadiusHandler(_config);
        if (_config.AutoClearWeeds.IsEnable) yield return new ClearWeedsHandler(_config);
        if (_config.AutoDigSpots.IsEnable) yield return new DigSpotHandler(_config);
        if (_config.AutoHarvestMachine.IsEnable) yield return new HarvestMachineHandler(_config);
        if (_config.AutoTriggerMachine.IsEnable) yield return new TriggerMachineHandler(_config);
        if (_config.AutoUseFairyDust.IsEnable) yield return new FairyDustHandler(_config);
        if (_config.AutoGarbageCan.IsEnable) yield return new GarbageCanHandler(_config);
        if (_config.AutoPlaceFloor.IsEnable) yield return new PlaceFloorHandler(_config);
    }
}