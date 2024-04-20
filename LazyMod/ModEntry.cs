using Common;
using LazyMod.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LazyMod;

public class ModEntry : Mod
{
    private ModConfig config = new();
    private AutomationManger? automationManger;
    private InfoManager? infoManager;

    public override void Entry(IModHelper helper)
    {
        // 读取配置文件
        config = helper.ReadConfig<ModConfig>();

        // 初始化
        I18n.Init(helper.Translation);
        automationManger = new AutomationManger(config);
        infoManager = new InfoManager(config);

        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.DayEnding += OnDayEnded;
        helper.Events.Display.RenderedHud += OnRenderedHud;
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        infoManager?.Draw(e.SpriteBatch);
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        automationManger?.OnDayStarted();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        automationManger?.Update();
    }

    private void OnDayEnded(object? sender, DayEndingEventArgs e)
    {
        automationManger?.OnDayEnded();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );

        configMenu.AddPageLink(
            ModManifest,
            "Farming",
            I18n.Config_FarmingPage_Name
        );

        configMenu.AddPageLink(
            ModManifest,
            "Animal",
            I18n.Config_AnimalPage_Name
        );

        configMenu.AddPageLink(
            ModManifest,
            "Mining",
            I18n.Config_MiningPage_Name
        );

        configMenu.AddPageLink(
            ModManifest,
            "Foraging",
            I18n.Config_ForagingPage_Name
        );

        configMenu.AddPageLink(
            ModManifest,
            "Food",
            I18n.Config_FoodPage_Name
        );

        configMenu.AddPageLink(
            ModManifest,
            "Other",
            I18n.Config_OtherPage_Name
        );

        #region 耕种

        configMenu.AddPage(
            ModManifest,
            "Farming",
            I18n.Config_FarmingPage_Name
        );
        // 自动耕地
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoTillDirt_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoTillDirt,
            value => config.AutoTillDirt = value,
            I18n.Config_AutoTillDirt_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoTillDirtRange,
            value => config.AutoTillDirtRange = value,
            I18n.Config_AutoTillDirtRange_Name,
            null,
            0,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopAutoTillDirtStamina,
            value => config.StopAutoTillDirtStamina = value,
            I18n.Config_StopAutoTillDirtStamina_Name
        );
        // 自动清理耕地
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearTilledDirt_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearTilledDirt,
            value => config.AutoClearTilledDirt = value,
            I18n.Config_AutoClearTilledDirt_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearTilledDirtRange,
            value => config.AutoClearTilledDirtRange = value,
            I18n.Config_AutoClearTilledDirtRange_Name,
            null,
            0,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopAutoClearTilledDirtStamina,
            value => config.StopAutoClearTilledDirtStamina = value,
            I18n.Config_StopAutoClearTilledDirtStamina_Name
        );
        // 自动浇水
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoWaterDirt_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoWaterDirt,
            value => config.AutoWaterDirt = value,
            I18n.Config_AutoWaterDirt_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoWaterDirtRange,
            value => config.AutoWaterDirtRange = value,
            I18n.Config_AutoWaterDirtRange_Name,
            null,
            0,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopAutoWaterDirtStamina,
            value => config.StopAutoWaterDirtStamina = value,
            I18n.Config_StopAutoWaterDirtStamina_Name
        );
        // 自动补充水壶
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoRefillWateringCan_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoRefillWateringCan,
            value => config.AutoRefillWateringCan = value,
            I18n.Config_AutoRefillWateringCan_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoRefillWateringCanRange,
            value => config.AutoRefillWateringCanRange = value,
            I18n.Config_AutoRefillWateringCanRange_Name,
            null,
            1,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindWateringCanFromInventory,
            value => config.FindWateringCanFromInventory = value,
            I18n.Config_FindWateringCanFromInventory_Name
        );
        // 自动播种
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoSeed_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoSeed,
            value => config.AutoSeed = value,
            I18n.Config_AutoSeed_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoSeedRange,
            value => config.AutoSeedRange = value,
            I18n.Config_AutoSeedRange_Name,
            null,
            0,
            3
        );
        // 自动施肥
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoFertilize_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoFertilize,
            value => config.AutoFertilize = value,
            I18n.Config_AutoFertilize_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoFertilizeRange,
            value => config.AutoFertilizeRange = value,
            I18n.Config_AutoFertilizeRange_Name,
            null,
            0,
            3
        );
        // 自动收获作物
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoHarvestCrop_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoHarvestCrop,
            value => config.AutoHarvestCrop = value,
            I18n.Config_AutoHarvestCrop_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoHarvestCropRange,
            value => config.AutoHarvestCropRange = value,
            I18n.Config_AutoHarvestCropRange_Name,
            null,
            0,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoHarvestFlower,
            value => config.AutoHarvestFlower = value,
            I18n.Config_AutoHarvestFlower_Name
        );
        // 自动摇晃果树
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoShakeFruitTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoShakeFruitTree,
            value => config.AutoShakeFruitTree = value,
            I18n.Config_AutoShakeFruitTree_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoShakeFruitTreeRange,
            value => config.AutoShakeFruitTreeRange = value,
            I18n.Config_AutoShakeFruitTreeRange_Name,
            null,
            1,
            3
        );
        // 自动清理枯萎作物
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearDeadCrop_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearDeadCrop,
            value => config.AutoClearDeadCrop = value,
            I18n.Config_AutoClearDeadCrop_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearDeadCropRange,
            value => config.AutoClearDeadCropRange = value,
            I18n.Config_AutoClearDeadCropRange_Name,
            null,
            0,
            3
        );

        #endregion

        #region 动物

        configMenu.AddPage(
            ModManifest,
            "Animal",
            I18n.Config_AnimalPage_Name
        );
        // 自动抚摸
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoPetAnimal_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoPetAnimal,
            value => config.AutoPetAnimal = value,
            I18n.Config_AutoPetAnimal_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoPetAnimalRange,
            value => config.AutoPetAnimalRange = value,
            I18n.Config_AutoPetAnimalRange_Name,
            null,
            1,
            3
        );
        // 自动挤奶
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoMilkAnimal_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoMilkAnimal,
            value => config.AutoMilkAnimal = value,
            I18n.Config_AutoMilkAnimal_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoMilkAnimalRange,
            value => config.AutoMilkAnimalRange = value,
            I18n.Config_AutoMilkAnimalRange_Name,
            null,
            1,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindMilkPailFromInventory,
            value => config.FindMilkPailFromInventory = value,
            I18n.Config_FindMilkPailFromInventory_Name
        );
        // 自动剪毛
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoShearsAnimal_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoShearsAnimal,
            value => config.AutoShearsAnimal = value,
            I18n.Config_AutoShearsAnimal_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoShearsAnimalRange,
            value => config.AutoShearsAnimalRange = value,
            I18n.Config_AutoShearsAnimalRange_Name,
            null,
            1,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindShearsFromInventory,
            value => config.FindShearsFromInventory = value,
            I18n.Config_FindShearsFromInventory_Name
        );
        // 自动打开动物门
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoOpenAnimalDoor_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoOpenAnimalDoor,
            value => config.AutoOpenAnimalDoor = value,
            I18n.Config_AutoOpenAnimalDoor_Name
        );
        // 自动打开栅栏门
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoOpenFenceGate_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoOpenFenceGate,
            value => config.AutoOpenFenceGate = value,
            I18n.Config_AutoOpenFenceGate_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoOpenFenceGateRange,
            value => config.AutoOpenFenceGateRange = value,
            I18n.Config_AutoOpenFenceGateRange_Name,
            null,
            1,
            3
        );
        // 自动抚摸宠物
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoPetPet_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoPetPet,
            value => config.AutoPetPet = value,
            I18n.Config_AutoPetPet_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoPetPetRange,
            value => config.AutoPetPetRange = value,
            I18n.Config_AutoPetPetRange_Name,
            null,
            1,
            3
        );

        #endregion

        #region 采矿

        configMenu.AddPage(
            ModManifest,
            "Mining",
            I18n.Config_MiningPage_Name
        );

        // 自动收集煤炭
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoCollectCoal_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoCollectCoal,
            value => config.AutoCollectCoal = value,
            I18n.Config_AutoCollectCoal_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoCollectCoalRange,
            value => config.AutoCollectCoalRange = value,
            I18n.Config_AutoCollectCoalRange_Name,
            null,
            1,
            3
        );
        // 自动破坏容器
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoBreakContainer_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoBreakContainer,
            value => config.AutoBreakContainer = value,
            I18n.Config_AutoBreakContainer_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoBreakContainerRange,
            value => config.AutoBreakContainerRange = value,
            I18n.Config_AutoBreakContainerRange_Name,
            null,
            1,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindWeaponFromInventory,
            value => config.FindWeaponFromInventory = value,
            I18n.Config_FindWeaponFromInventory_Name
        );
        // 自动打开宝箱
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoOpenTreasure_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoOpenTreasure,
            value => config.AutoOpenTreasure = value,
            I18n.Config_AutoOpenTreasure_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoOpenTreasureRange,
            value => config.AutoOpenTreasureRange = value,
            I18n.Config_AutoOpenTreasureRange_Name,
            null,
            1,
            3
        );
        // 自动清理水晶
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearCrystal_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearCrystal,
            value => config.AutoClearCrystal = value,
            I18n.Config_AutoClearCrystal_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearCrystalRange,
            value => config.AutoClearCrystalRange = value,
            I18n.Config_AutoClearCrystalRange_Name,
            null,
            1,
            3
        );
        // 显示矿井信息
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_ShowMineShaftInfo_Name
        );
        // 显示梯子信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowLadderInfo,
            value => config.ShowLadderInfo = value,
            I18n.Config_ShowLadderInfo_Name
        );
        // 显示竖井信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowShaftInfo,
            value => config.ShowShaftInfo = value,
            I18n.Config_ShowShaftInfo_Name
        );
        // 显示怪物信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowMonsterInfo,
            value => config.ShowMonsterInfo = value,
            I18n.Config_ShowMonsterInfo_Name
        );
        // 显示矿物信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowMineralInfo,
            value => config.ShowMineralInfo = value,
            I18n.Config_ShowMineralInfo_Name
        );

        #endregion

        #region 觅食

        configMenu.AddPage(
            ModManifest,
            "Foraging",
            I18n.Config_ForagingPage_Name
        );
        // 自动觅食
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoForage_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoForage,
            value => config.AutoForage = value,
            I18n.Config_AutoForage_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoForageRange,
            value => config.AutoForageRange = value,
            I18n.Config_AutoForageRange_Name,
            null,
            0,
            3
        );
        // 自动摇树
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoShakeTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoShakeTree,
            value => config.AutoShakeTree = value,
            I18n.Config_AutoShakeTree_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoShakeTreeRange,
            value => config.AutoShakeTreeRange = value,
            I18n.Config_AutoShakeTreeRange_Name,
            null,
            1,
            3
        );
        // 自动收获苔藓
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoHarvestMoss_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoHarvestMoss,
            value => config.AutoHarvestMoss = value,
            I18n.Config_AutoHarvestMoss_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoHarvestMossRange,
            value => config.AutoHarvestMossRange = value,
            I18n.Config_AutoHarvestMossRange_Name,
            null,
            1,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindScytheFromInventory,
            value => config.FindScytheFromInventory = value,
            I18n.Config_FindScytheFromInventory_Name
        );
        // 自动清理树枝
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearTwig_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearTwig,
            value => config.AutoClearTwig = value,
            I18n.Config_AutoClearTwig_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearTwigRange,
            value => config.AutoClearTwigRange = value,
            I18n.Config_AutoClearTwigRange_Name,
            null,
            1,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopAutoClearTwigStamina,
            value => config.StopAutoClearTwigStamina = value,
            I18n.Config_StopAutoClearTwigStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindAxeFromInventory,
            value => config.FindAxeFromInventory = value,
            I18n.Config_FindAxeFromInventory_Name
        );
        // 自动清理树种
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearTreeSeed_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearTreeSeed,
            value => config.AutoClearTreeSeed = value,
            I18n.Config_AutoClearTreeSeed_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearTreeSeedRange,
            value => config.AutoClearTreeSeedRange = value,
            I18n.Config_AutoClearTreeSeedRange_Name,
            null,
            0,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopAutoClearTreeSeedStamina,
            value => config.StopAutoClearTreeSeedStamina = value,
            I18n.Config_StopAutoClearTreeSeedStamina_Name
        );

        #endregion

        #region 食物

        configMenu.AddPage(
            ModManifest,
            "Food",
            I18n.Config_FoodPage_Name
        );
        // 自动吃食物-体力
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoEatFoodForStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoEatFoodForStamina,
            value => config.AutoEatFoodForStamina = value,
            I18n.Config_AutoEatFoodForStamina_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoEatFoodStaminaRate,
            value => config.AutoEatFoodStaminaRate = value,
            I18n.Config_AutoEatFoodStaminaRate_Name,
            null,
            0.05f,
            0.95f,
            0.05f
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.IntelligentFoodSelectionForStamina,
            value => config.IntelligentFoodSelectionForStamina = value,
            I18n.Config_IntelligentFoodSelectionForStamina_Name
        );
        // 自动吃食物-生命值
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoEatFoodForHealth_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoEatFoodForHealth,
            value => config.AutoEatFoodForHealth = value,
            I18n.Config_AutoEatFoodForHealth_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoEatFoodHealthRate,
            value => config.AutoEatFoodHealthRate = value,
            I18n.Config_AutoEatFoodHealthRate_Name,
            null,
            0.05f,
            0.95f,
            0.05f
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.IntelligentFoodSelectionForHealth,
            value => config.IntelligentFoodSelectionForHealth = value,
            I18n.Config_IntelligentFoodSelectionForHealth_Name
        );
        // 自动吃食物-Buff
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoEatFoodForBuff_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoEatFoodForBuff,
            value => config.AutoEatFoodForBuff = value,
            I18n.Config_AutoEatFoodForBuff_Name
        );
        // 自动喝饮料-Buff
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoDrinkForBuff_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoDrinkForBuff,
            value => config.AutoDrinkForBuff = value,
            I18n.Config_AutoDrinkForBuff_Name
        );

        #endregion

        #region 其他

        configMenu.AddPage(
            ModManifest,
            "Other",
            I18n.Config_OtherPage_Name
        );
        // 自动清理石头
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearStone_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearStone,
            value => config.AutoClearStone = value,
            I18n.Config_AutoClearStone_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearStoneRange,
            value => config.AutoClearStoneRange = value,
            I18n.Config_AutoClearStoneRange_Name,
            null,
            1,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopAutoClearStoneStamina,
            value => config.StopAutoClearStoneStamina = value,
            I18n.Config_StopAutoClearStoneStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindPickaxeFromInventory,
            value => config.FindPickaxeFromInventory = value,
            I18n.Config_FindPickaxeFromInventory_Name
        );
        // 自动清理杂草
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearWeeds_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearWeeds,
            value => config.AutoClearWeeds = value,
            I18n.Config_AutoClearWeeds_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearWeedsRange,
            value => config.AutoClearWeedsRange = value,
            I18n.Config_AutoClearWeedsRange_Name,
            null,
            1,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindToolFromInventory,
            value => config.FindToolFromInventory = value,
            I18n.Config_FindToolFromInventory_Name
        );
        // 自动挖掘远古斑点
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoDigArtifactSpots_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoDigArtifactSpots,
            value => config.AutoDigArtifactSpots = value,
            I18n.Config_AutoDigArtifactSpots_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoDigArtifactSpotsRange,
            value => config.AutoDigArtifactSpotsRange = value,
            I18n.Config_AutoDigArtifactSpotsRange_Name,
            null,
            0,
            3);
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopAutoDigArtifactSpotsStamina,
            value => config.StopAutoDigArtifactSpotsStamina = value,
            I18n.Config_StopAutoDigArtifactSpotsStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindHoeFromInventory,
            value => config.FindHoeFromInventory = value,
            I18n.Config_FindHoeFromInventory_Name
        );
        // 自动收获机器
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoHarvestMachine_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoHarvestMachine,
            value => config.AutoHarvestMachine = value,
            I18n.Config_AutoHarvestMachine_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoHarvestMachineRange,
            value => config.AutoHarvestMachineRange = value,
            I18n.Config_AutoHarvestMachineRange_Name,
            null,
            1,
            3
        );
        // 自动触发机器
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoTriggerMachine_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoTriggerMachine,
            value => config.AutoTriggerMachine = value,
            I18n.Config_AutoTriggerMachine_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoTriggerMachineRange,
            value => config.AutoTriggerMachineRange = value,
            I18n.Config_AutoTriggerMachineRange_Name,
            null,
            1,
            3
        );
        // 自动翻垃圾桶
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoGarbageCan_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoGarbageCan,
            value => config.AutoGarbageCan = value,
            I18n.Config_AutoGarbageCan_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoGarbageCanRange,
            value => config.AutoGarbageCanRange = value,
            I18n.Config_AutoGarbageCanRange_Name,
            null,
            1,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.StopAutoGarbageCanNearVillager,
            value => config.StopAutoGarbageCanNearVillager = value,
            I18n.Config_StopAutoGarbageCanNearVillager_Name
        );
        // 自动学习食谱
        // configMenu.AddBoolOption(
        //     ModManifest,
        //     () => config.AutoStudyRecipe,
        //     value => config.AutoStudyRecipe = value,
        //     I18n.Config_AutoStudyRecipe_Name
        // );

        #endregion
    }
}