using Common;
using LazyMod.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;


namespace LazyMod;

public class ModEntry : Mod
{
    private ModConfig config = new();
    private LazyModManager? lazyModManager;

    public override void Entry(IModHelper helper)
    {
        // 读取配置文件
        config = helper.ReadConfig<ModConfig>();

        // 初始化
        I18n.Init(helper.Translation);
        lazyModManager = new LazyModManager(config);

        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.DayEnding += OnDayEnded;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        lazyModManager?.OnDayStarted();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        lazyModManager?.Update();
    }

    private void OnDayEnded(object? sender, DayEndingEventArgs e)
    {
        lazyModManager?.OnDayEnded();
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
            I18n.Config_AutoCleanDeadCrop_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoCleanDeadCrop,
            value => config.AutoCleanDeadCrop = value,
            I18n.Config_AutoCleanDeadCrop_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoCleanDeadCropRange,
            value => config.AutoCleanDeadCropRange = value,
            I18n.Config_AutoCleanDeadCropRange_Name,
            null,
            0,
            3
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindScytheFromInventory,
            value => config.FindScytheFromInventory = value,
            I18n.Config_FindScytheFromInventory_Name
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
            I18n.Config_AutoCollectCoalRange_Name
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
            I18n.Config_AutoBreakContainerRange_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindWeaponFromInventory,
            value => config.FindWeaponFromInventory = value,
            I18n.Config_FindWeaponFromInventory_Name
        );
        // 自动收集奖励
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoCollectReward_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoCollectReward,
            value => config.AutoCollectReward = value,
            I18n.Config_AutoCollectReward_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoCollectRewardRange,
            value => config.AutoCollectRewardRange = value,
            I18n.Config_AutoCollectRewardRange_Name
        );

        #endregion

        #region 其他

        configMenu.AddPage(
            ModManifest,
            "Other",
            I18n.Config_OtherPage_Name
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

        #endregion
    }
}