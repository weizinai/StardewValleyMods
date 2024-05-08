using Common.Integrations;
using LazyMod.Framework;
using LazyMod.Framework.Automation;
using LazyMod.Framework.Hud;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LazyMod;

public class ModEntry : Mod
{
    private ModConfig config = new();
    private AutomationManger automationManger = null!;
    private MiningHud miningHud = null!;

    public override void Entry(IModHelper helper)
    {
        // 读取配置文件
        config = helper.ReadConfig<ModConfig>();

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

        var buffMaintainAllowValues = new[]
            { "Combat", "Farming", "Fishing", "Mining", "Luck", "Foraging", "MaxStamina", "MagneticRadius", "Speed", "Defense", "Attack", "None" };
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));

        configMenu.AddKeybindList(
            ModManifest,
            () => config.ToggleModStateKeybind,
            value => config.ToggleModStateKeybind = value,
            I18n.Config_ToggleModStateKeybind_Name,
            I18n.Config_ToggleModStateKeybind_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.Cooldown,
            value => config.Cooldown = value,
            I18n.Config_Cooldown_Name,
            I18n.Config_Cooldown_Tooltip,
            0,
            60,
            5
        );
        configMenu.AddPageLink(ModManifest, "Farming", I18n.Config_FarmingPage_Name);
        configMenu.AddPageLink(ModManifest, "Animal", I18n.Config_AnimalPage_Name);
        configMenu.AddPageLink(ModManifest, "Mining", I18n.Config_MiningPage_Name);
        configMenu.AddPageLink(ModManifest, "Foraging", I18n.Config_ForagingPage_Name);
        configMenu.AddPageLink(ModManifest, "Fishing", I18n.Config_FishingPage_Name);
        configMenu.AddPageLink(ModManifest, "Food", I18n.Config_FoodPage_Name);
        configMenu.AddPageLink(ModManifest, "Other", I18n.Config_OtherPage_Name);

        #region 耕种

        configMenu.AddPage(
            ModManifest,
            "Farming",
            I18n.Config_FarmingPage_Name
        );
        // 自动耕地
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoTillDirt_Name,
            I18n.Config_AutoTillDirt_Tooltip
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
            () => config.StopTillDirtStamina,
            value => config.StopTillDirtStamina = value,
            I18n.Config_StopTillDirtStamina_Name
        );
        // 自动清理耕地
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearTilledDirt_Name,
            I18n.Config_AutoClearTilledDirt_Tooltip
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
            () => config.StopClearTilledDirtStamina,
            value => config.StopClearTilledDirtStamina = value,
            I18n.Config_StopClearTilledDirtStamina_Name
        );
        // 自动浇水
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoWaterDirt_Name,
            I18n.Config_AutoWaterDirt_Tooltip
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
            () => config.StopWaterDirtStamina,
            value => config.StopWaterDirtStamina = value,
            I18n.Config_StopWaterDirtStamina_Name
        );
        // 自动补充水壶
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoRefillWateringCan_Name,
            I18n.Config_AutoRefillWateringCan_Tooltip
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
            I18n.Config_FindWateringCanFromInventory_Name,
            I18n.Config_FindWateringCanFromInventory_Tooltip
        );
        // 自动播种
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoSeed_Name,
            I18n.Config_AutoSeed_Tooltip
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
            I18n.Config_AutoFertilize_Name,
            I18n.Config_AutoFertilize_Tooltip
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
            I18n.Config_AutoClearDeadCrop_Name,
            I18n.Config_AutoClearDeadCrop_Tooltip
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
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindToolForClearDeadCrop,
            value => config.FindToolForClearDeadCrop = value,
            I18n.Config_FindToolForClearDeadCrop_Name,
            I18n.Config_FindToolForClearDeadCrop_Tooltip
        );

        #endregion

        #region 动物

        configMenu.AddPage(
            ModManifest,
            "Animal",
            I18n.Config_AnimalPage_Name
        );
        // 自动抚摸动物
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
        // 自动挤奶
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoMilkAnimal_Name,
            I18n.Config_AutoMilkAnimal_Tooltip
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
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopMilkAnimalStamina,
            value => config.StopMilkAnimalStamina = value,
            I18n.Config_StopMilkAnimalStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindMilkPailFromInventory,
            value => config.FindMilkPailFromInventory = value,
            I18n.Config_FindMilkPailFromInventory_Name,
            I18n.Config_FindMilkPailFromInventory_Tooltip
        );
        // 自动剪毛
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoShearsAnimal_Name,
            I18n.Config_AutoShearsAnimal_Tooltip
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
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopShearsAnimalStamina,
            value => config.StopShearsAnimalStamina = value,
            I18n.Config_StopShearsAnimalStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindShearsFromInventory,
            value => config.FindShearsFromInventory = value,
            I18n.Config_FindShearsFromInventory_Name,
            I18n.Config_FindShearsFromInventory_Tooltip
        );
        // 自动喂食动物饼干
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoFeedAnimalCracker_Name,
            I18n.Config_AutoFeedAnimalCracker_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoFeedAnimalCracker,
            value => config.AutoFeedAnimalCracker = value,
            I18n.Config_AutoFeedAnimalCracker_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoFeedAnimalCrackerRange,
            value => config.AutoFeedAnimalCrackerRange = value,
            I18n.Config_AutoFeedAnimalCrackerRange_Name,
            null,
            1,
            3
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
            I18n.Config_AutoOpenAnimalDoor_Name,
            I18n.Config_AutoOpenAnimalDoor_Tooltip
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

        #endregion

        #region 采矿

        configMenu.AddPage(
            ModManifest,
            "Mining",
            I18n.Config_MiningPage_Name
        );
        // 自动清理石头
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearStone_Name,
            I18n.Config_AutoClearStone_Tooltip
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
            () => config.StopClearStoneStamina,
            value => config.StopClearStoneStamina = value,
            I18n.Config_StopClearStoneStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindPickaxeFromInventory,
            value => config.FindPickaxeFromInventory = value,
            I18n.Config_FindPickaxeFromInventory_Name,
            I18n.Config_FindPickaxeFromInventory_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearStoneOnMineShaft,
            value => config.ClearStoneOnMineShaft = value,
            I18n.Config_ClearStoneOnMineShaft_Name,
            I18n.Config_ClearStoneOnMineShaft_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearStoneOnVolcano,
            value => config.ClearStoneOnVolcano = value,
            I18n.Config_ClearStoneOnVolcano_Name,
            I18n.Config_ClearStoneOnVolcano_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearFarmStone,
            value => config.ClearFarmStone = value,
            I18n.Config_ClearFarmStone_Name,
            I18n.Config_ClearFarmStone_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearOtherStone,
            value => config.ClearOtherStone = value,
            I18n.Config_ClearOtherStone_Name,
            I18n.Config_ClearOtherStone_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearIslandStone,
            value => config.ClearIslandStone = value,
            I18n.Config_ClearIslandStone_Name,
            I18n.Config_ClearIslandStone_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearOreStone,
            value => config.ClearOreStone = value,
            I18n.Config_ClearOreStone_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearGemStone,
            value => config.ClearGemStone = value,
            I18n.Config_ClearGemStone_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearGeodeStone,
            value => config.ClearGeodeStone = value,
            I18n.Config_ClearGeodeStone_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearBoulder,
            value => config.ClearBoulder = value,
            I18n.Config_ClearBoulder_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearMeteorite,
            value => config.ClearMeteorite = value,
            I18n.Config_ClearMeteorite_Name
        );
        // 自动收集煤炭
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoCollectCoal_Name,
            I18n.Config_AutoCollectCoal_Tooltip
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
            I18n.Config_AutoBreakContainer_Name,
            I18n.Config_AutoBreakContainer_Tooltip
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
            I18n.Config_FindWeaponFromInventory_Name,
            I18n.Config_FindWeaponFromInventory_Tooltip
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
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindToolForClearCrystal,
            value => config.FindToolForClearCrystal = value,
            I18n.Config_FindToolForClearCrystal_Name,
            I18n.Config_FindToolForClearCrystal_Tooltip
        );
        // 自动冷却岩浆
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoCoolLava_Name,
            I18n.Config_AutoCoolLava_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoCoolLava,
            value => config.AutoCoolLava = value,
            I18n.Config_AutoCoolLava_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoCoolLavaRange,
            value => config.AutoCoolLavaRange = value,
            I18n.Config_AutoCoolLavaRange_Name,
            null,
            1,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopCoolLavaStamina,
            value => config.StopCoolLavaStamina = value,
            I18n.Config_StopCoolLavaStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindToolForCoolLava,
            value => config.FindToolForCoolLava = value,
            I18n.Config_FindToolForCoolLava_Name,
            I18n.Config_FindToolForCoolLava_Tooltip
        );
        // 显示矿井信息
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_ShowMineShaftInfo_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowLadderInfo,
            value => config.ShowLadderInfo = value,
            I18n.Config_ShowLadderInfo_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowShaftInfo,
            value => config.ShowShaftInfo = value,
            I18n.Config_ShowShaftInfo_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowMonsterInfo,
            value => config.ShowMonsterInfo = value,
            I18n.Config_ShowMonsterInfo_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowMonsterKillInfo,
            value => config.ShowMonsterKillInfo = value,
            I18n.Config_ShowMonsterKillInfo_Name,
            I18n.Config_ShowMonsterKillInfo_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowMineralInfo,
            value => config.ShowMineralInfo = value,
            I18n.Config_ShowMineralInfo_Name
        );

        #endregion

        #region 采集

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
        // 自动砍树
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoChopTree_Name,
            I18n.Config_AutoChopTree_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoChopTree,
            value => config.AutoChopTree = value,
            I18n.Config_AutoChopTree_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoChopTreeRange,
            value => config.AutoChopTreeRange = value,
            I18n.Config_AutoChopTreeRange_Name,
            null,
            0,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopChopTreeStamina,
            value => config.StopChopTreeStamina = value,
            I18n.Config_StopChopTreeStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopTapperTree,
            value => config.ChopTapperTree = value,
            I18n.Config_ChopTapperTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopVinegarTree,
            value => config.ChopVinegarTree = value,
            I18n.Config_ChopVinegarTree_Name
        );
        configMenu.AddPageLink(
            ModManifest,
            "TreeSettings",
            I18n.Config_TreeSettingsPage_Name
        );
        // 自动收获姜
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoHarvestGinger_Name,
            I18n.Config_AutoHarvestGinger_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoHarvestGinger,
            value => config.AutoHarvestGinger = value,
            I18n.Config_AutoHarvestGinger_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoHarvestGingerRange,
            value => config.AutoHarvestGingerRange = value,
            I18n.Config_AutoHarvestGingerRange_Name,
            null,
            0,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopHarvestGingerStamina,
            value => config.StopHarvestGingerStamina = value,
            I18n.Config_StopHarvestGingerStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindToolForHarvestGinger,
            value => config.FindToolForHarvestGinger = value,
            I18n.Config_FindToolForHarvestGinger_Name,
            I18n.Config_FindToolForHarvestGinger_Tooltip
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
            0,
            3
        );
        // 自动收获苔藓
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoHarvestMoss_Name,
            I18n.Config_AutoHarvestMoss_Tooltip
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
            I18n.Config_FindScytheFromInventory_Name,
            I18n.Config_FindScytheFromInventory_Tooltip
        );
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoPlaceTapper_Name,
            I18n.Config_AutoPlaceTapper_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoPlaceTapper,
            value => config.AutoPlaceTapper = value,
            I18n.Config_AutoPlaceTapper_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoPlaceTapperRange,
            value => config.AutoPlaceTapperRange = value,
            I18n.Config_AutoPlaceTapperRange_Name,
            null,
            1,
            3
        );
        // 自动在树上浇醋
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoPlaceVinegar_Name,
            I18n.Config_AutoPlaceVinegar_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoPlaceVinegar,
            value => config.AutoPlaceVinegar = value,
            I18n.Config_AutoPlaceVinegar_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoPlaceVinegarRange,
            value => config.AutoPlaceVinegarRange = value,
            I18n.Config_AutoPlaceVinegarRange_Name,
            null,
            1,
            3
        );
        // 自动清理树枝
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearWood_Name,
            I18n.Config_AutoClearWood_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoClearWood,
            value => config.AutoClearWood = value,
            I18n.Config_AutoClearWood_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoClearWoodRange,
            value => config.AutoClearWoodRange = value,
            I18n.Config_AutoClearWoodRange_Name,
            null,
            1,
            3
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopClearWoodStamina,
            value => config.StopClearWoodStamina = value,
            I18n.Config_StopClearWoodStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindAxeFromInventory,
            value => config.FindAxeFromInventory = value,
            I18n.Config_FindAxeFromInventory_Name,
            I18n.Config_FindAxeFromInventory_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearTwig,
            value => config.ClearTwig = value,
            I18n.Config_ClearTwig_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearStump,
            value => config.ClearStump = value,
            I18n.Config_ClearStump_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearHollowLog,
            value => config.ClearHollowLog = value,
            I18n.Config_ClearHollowLog_Name
        );

        #endregion

        #region 钓鱼

        configMenu.AddPage(
            ModManifest,
            "Fishing",
            I18n.Config_FishingPage_Name
        );
        // 自动使用蟹笼
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoPlaceCarbPot_Name,
            I18n.Config_AutoPlaceCarbPot_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoPlaceCarbPot,
            value => config.AutoPlaceCarbPot = value,
            I18n.Config_AutoPlaceCarbPot_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoPlaceCarbPotRange,
            value => config.AutoPlaceCarbPotRange = value,
            I18n.Config_AutoPlaceCarbPotRange_Name,
            null,
            1,
            3
        );
        // 自动添加蟹笼鱼饵
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoAddBaitForCarbPot_Name,
            I18n.Config_AutoAddBaitForCarbPot_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoAddBaitForCarbPot,
            value => config.AutoAddBaitForCarbPot = value,
            I18n.Config_AutoAddBaitForCarbPot_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoAddBaitForCarbPotRange,
            value => config.AutoAddBaitForCarbPotRange = value,
            I18n.Config_AutoAddBaitForCarbPotRange_Name,
            null,
            1,
            3
        );
        // 自动收获蟹笼
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoHarvestCarbPot_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoHarvestCarbPot,
            value => config.AutoHarvestCarbPot = value,
            I18n.Config_AutoHarvestCarbPot_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoHarvestCarbPotRange,
            value => config.AutoHarvestCarbPotRange = value,
            I18n.Config_AutoHarvestCarbPotRange_Name,
            null,
            1,
            3
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
            I18n.Config_IntelligentFoodSelectionForStamina_Name,
            I18n.Config_IntelligentFoodSelectionForStamina_Tooltip
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
            I18n.Config_IntelligentFoodSelectionForHealth_Name,
            I18n.Config_IntelligentFoodSelectionForHealth_Tooltip
        );
        // 自动吃增益食物
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoEatBuffFood_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoEatBuffFood,
            value => config.AutoEatBuffFood = value,
            I18n.Config_AutoEatBuffFood_Name,
            I18n.Config_AutoEatBuffFood_Tooltip
        );
        configMenu.AddTextOption(
            ModManifest,
            () => config.FoodBuffMaintain1.ToString(),
            value => config.FoodBuffMaintain1 = AutoFood.GetBuffType(value),
            I18n.Config_FoodBuffMaintain1_Name,
            I18n.Config_FoodBuffMaintain_Tooltip,
            buffMaintainAllowValues,
            GetStringFromBuffType
        );
        configMenu.AddTextOption(
            ModManifest,
            () => config.FoodBuffMaintain2.ToString(),
            value => config.FoodBuffMaintain2 = AutoFood.GetBuffType(value),
            I18n.Config_FoodBuffMaintain2_Name,
            I18n.Config_FoodBuffMaintain_Tooltip,
            buffMaintainAllowValues,
            GetStringFromBuffType
        );
        // 自动喝增益饮料
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoDrinkBuffDrink_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoDrinkBuffDrink,
            value => config.AutoDrinkBuffDrink = value,
            I18n.Config_AutoDrinkBuffDrink_Name,
            I18n.Config_AutoDrinkBuffDrink_Tooltip
        );
        configMenu.AddTextOption(
            ModManifest,
            () => config.DrinkBuffMaintain1.ToString(),
            value => config.DrinkBuffMaintain1 = AutoFood.GetBuffType(value),
            I18n.Config_DrinkBuffMaintain1_Name,
            I18n.Config_DrinkBuffMaintain_ToolTip,
            buffMaintainAllowValues,
            GetStringFromBuffType
        );
        configMenu.AddTextOption(
            ModManifest,
            () => config.DrinkBuffMaintain2.ToString(),
            value => config.DrinkBuffMaintain2 = AutoFood.GetBuffType(value),
            I18n.Config_DrinkBuffMaintain2_Name,
            I18n.Config_DrinkBuffMaintain_ToolTip,
            buffMaintainAllowValues,
            GetStringFromBuffType
        );

        #endregion

        #region 其他

        configMenu.AddPage(
            ModManifest,
            "Other",
            I18n.Config_OtherPage_Name
        );
        // 磁力范围增加
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_MagneticRadiusIncrease_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.MagneticRadiusIncrease,
            value => config.MagneticRadiusIncrease = value,
            I18n.Config_MagneticRadiusIncrease_Name,
            null,
            0,
            10
        );
        // 自动清理杂草
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoClearWeeds_Name,
            I18n.Config_AutoClearWeeds_Tooltip
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
            () => config.FindToolForClearWeeds,
            value => config.FindToolForClearWeeds = value,
            I18n.Config_FindToolForClearWeeds_Name,
            I18n.Config_FindToolForClearWeeds_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ClearLargeWeeds,
            value => config.ClearLargeWeeds = value,
            I18n.Config_ClearLargeWeeds_Name
        );
        // 自动挖掘远古斑点
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoDigSpots_Name,
            I18n.Config_AutoDigSpots_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoDigSpots,
            value => config.AutoDigSpots = value,
            I18n.Config_AutoDigSpots_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoDigSpotsRange,
            value => config.AutoDigSpotsRange = value,
            I18n.Config_AutoDigSpotsRange_Name,
            null,
            0,
            3);
        configMenu.AddNumberOption(
            ModManifest,
            () => config.StopDigSpotsStamina,
            value => config.StopDigSpotsStamina = value,
            I18n.Config_StopDigSpotsStamina_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.FindHoeFromInventory,
            value => config.FindHoeFromInventory = value,
            I18n.Config_FindHoeFromInventory_Name,
            I18n.Config_FindHoeFromInventory_Tooltip
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
            () => config.StopGarbageCanNearVillager,
            value => config.StopGarbageCanNearVillager = value,
            I18n.Config_StopGarbageCanNearVillager_Name
        );
        // 自动放置地板
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_AutoPlaceFloor_Name,
            I18n.Config_AutoPlaceFloor_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.AutoPlaceFloor,
            value => config.AutoPlaceFloor = value,
            I18n.Config_AutoPlaceFloor_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.AutoPlaceFloorRange,
            value => config.AutoPlaceFloorRange = value,
            I18n.Config_AutoPlaceFloorRange_Name,
            null,
            0,
            3
        );
        // 自动学习食谱
        // configMenu.AddBoolOption(
        //     ModManifest,
        //     () => config.AutoStudyRecipe,
        //     value => config.AutoStudyRecipe = value,
        //     I18n.Config_AutoStudyRecipe_Name
        // );

        #endregion

        #region 树设置

        configMenu.AddPage(ModManifest, "TreeSettings", I18n.Config_TreeSettingsPage_Name);
        // 橡树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_OakTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopOakTree[0],
            value => config.ChopOakTree[0] = value,
            I18n.Config_ChopSeedStageOakTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopOakTree[1],
            value => config.ChopOakTree[1] = value,
            I18n.Config_ChopSproutStageOakTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopOakTree[2],
            value => config.ChopOakTree[2] = value,
            I18n.Config_ChopSaplingStageOakTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopOakTree[3],
            value => config.ChopOakTree[3] = value,
            I18n.Config_ChopBushStageOakTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopOakTree[4],
            value => config.ChopOakTree[4] = value,
            I18n.Config_ChopSmallTreeStageOakTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopOakTree[5],
            value => config.ChopOakTree[5] = value,
            I18n.Config_ChopOakTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopOakTree[-1],
            value => config.ChopOakTree[-1] = value,
            I18n.Config_ChopStumpStageOakTree_Name
        );
        // 枫树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_MapleTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMapleTree[0],
            value => config.ChopMapleTree[0] = value,
            I18n.Config_ChopSeedStageMapleTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMapleTree[1],
            value => config.ChopMapleTree[1] = value,
            I18n.Config_ChopSproutStageMapleTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMapleTree[2],
            value => config.ChopMapleTree[2] = value,
            I18n.Config_ChopSaplingStageMapleTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMapleTree[3],
            value => config.ChopMapleTree[3] = value,
            I18n.Config_ChopBushStageMapleTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMapleTree[4],
            value => config.ChopMapleTree[4] = value,
            I18n.Config_ChopSmallTreeStageMapleTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMapleTree[5],
            value => config.ChopMapleTree[5] = value,
            I18n.Config_ChopMapleTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMapleTree[-1],
            value => config.ChopMapleTree[-1] = value,
            I18n.Config_ChopStumpStageMapleTree_Name
        );
        // 松树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_PineTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPineTree[0],
            value => config.ChopPineTree[0] = value,
            I18n.Config_ChopSeedStagePineTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPineTree[1],
            value => config.ChopPineTree[1] = value,
            I18n.Config_ChopSproutStagePineTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPineTree[2],
            value => config.ChopPineTree[2] = value,
            I18n.Config_ChopSaplingStagePineTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPineTree[3],
            value => config.ChopPineTree[3] = value,
            I18n.Config_ChopBushStagePineTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPineTree[4],
            value => config.ChopPineTree[4] = value,
            I18n.Config_ChopSmallTreeStagePineTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPineTree[5],
            value => config.ChopPineTree[5] = value,
            I18n.Config_ChopPineTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPineTree[-1],
            value => config.ChopPineTree[-1] = value,
            I18n.Config_ChopStumpStagePineTree_Name
        );
        // 桃花心木树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_MahoganyTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMahoganyTree[0],
            value => config.ChopMahoganyTree[0] = value,
            I18n.Config_ChopSeedStageMahoganyTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMahoganyTree[1],
            value => config.ChopMahoganyTree[1] = value,
            I18n.Config_ChopSproutStageMahoganyTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMahoganyTree[2],
            value => config.ChopMahoganyTree[2] = value,
            I18n.Config_ChopSaplingStageMahoganyTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMahoganyTree[3],
            value => config.ChopMahoganyTree[3] = value,
            I18n.Config_ChopBushStageMahoganyTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMahoganyTree[4],
            value => config.ChopMahoganyTree[4] = value,
            I18n.Config_ChopSmallTreeStageMahoganyTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMahoganyTree[5],
            value => config.ChopMahoganyTree[5] = value,
            I18n.Config_ChopMahoganyTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMahoganyTree[-1],
            value => config.ChopMahoganyTree[-1] = value,
            I18n.Config_ChopStumpStageMahoganyTree_Name
        );
        // 棕榈树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_PalmTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPalmTree[2],
            value => config.ChopPalmTree[2] = value,
            I18n.Config_ChopSaplingStagePalmTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPalmTree[3],
            value => config.ChopPalmTree[3] = value,
            I18n.Config_ChopBushStagePalmTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPalmTree[4],
            value => config.ChopPalmTree[4] = value,
            I18n.Config_ChopSmallTreeStagePalmTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPalmTree[5],
            value => config.ChopPalmTree[5] = value,
            I18n.Config_ChopPalmTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopPalmTree[-1],
            value => config.ChopPalmTree[-1] = value,
            I18n.Config_ChopStumpStagePalmTree_Name
        );
        // 蘑菇树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_MushroomTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMushroomTree[0],
            value => config.ChopMushroomTree[0] = value,
            I18n.Config_ChopSeedStageMushroomTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMushroomTree[1],
            value => config.ChopMushroomTree[1] = value,
            I18n.Config_ChopSproutStageMushroomTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMushroomTree[2],
            value => config.ChopMushroomTree[2] = value,
            I18n.Config_ChopSaplingStageMushroomTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMushroomTree[3],
            value => config.ChopMushroomTree[3] = value,
            I18n.Config_ChopBushStageMushroomTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMushroomTree[4],
            value => config.ChopMushroomTree[4] = value,
            I18n.Config_ChopSmallTreeStageMushroomTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMushroomTree[5],
            value => config.ChopMushroomTree[5] = value,
            I18n.Config_ChopMushroomTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMushroomTree[-1],
            value => config.ChopMushroomTree[-1] = value,
            I18n.Config_ChopStumpStageMushroomTree_Name
        );
        // 苔雨树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_GreenRainTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopGreenRainTree[0],
            value => config.ChopGreenRainTree[0] = value,
            I18n.Config_ChopSeedStageGreenRainTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopGreenRainTree[1],
            value => config.ChopGreenRainTree[1] = value,
            I18n.Config_ChopSproutStageGreenRainTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopGreenRainTree[2],
            value => config.ChopGreenRainTree[2] = value,
            I18n.Config_ChopSaplingStageGreenRainTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopGreenRainTree[3],
            value => config.ChopGreenRainTree[3] = value,
            I18n.Config_ChopBushStageGreenRainTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopGreenRainTree[4],
            value => config.ChopGreenRainTree[4] = value,
            I18n.Config_ChopSmallTreeStageGreenRainTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopGreenRainTree[5],
            value => config.ChopGreenRainTree[5] = value,
            I18n.Config_ChopGreenRainTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopGreenRainTree[-1],
            value => config.ChopGreenRainTree[-1] = value,
            I18n.Config_ChopStumpStageGreenRainTree_Name
        );
        // 神秘树
        configMenu.AddSectionTitle(ModManifest, I18n.Config_MysticTreeTitle_Name);
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMysticTree[0],
            value => config.ChopMysticTree[0] = value,
            I18n.Config_ChopSeedStageMysticTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMysticTree[1],
            value => config.ChopMysticTree[1] = value,
            I18n.Config_ChopSproutStageMysticTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMysticTree[2],
            value => config.ChopMysticTree[2] = value,
            I18n.Config_ChopSaplingStageMysticTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMysticTree[3],
            value => config.ChopMysticTree[3] = value,
            I18n.Config_ChopBushStageMysticTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMysticTree[4],
            value => config.ChopMysticTree[4] = value,
            I18n.Config_ChopSmallTreeStageMysticTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMysticTree[5],
            value => config.ChopMysticTree[5] = value,
            I18n.Config_ChopMysticTree_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ChopMysticTree[-1],
            value => config.ChopMysticTree[-1] = value,
            I18n.Config_ChopStumpStageMysticTree_Name
        );

        #endregion
    }

    private static string GetStringFromBuffType(string value)
    {
        return value switch
        {
            "Combat" => I18n.BuffType_Combat_Name(),
            "Farming" => I18n.BuffType_Farming_Name(),
            "Fishing" => I18n.BuffType_Fishing_Name(),
            "Mining" => I18n.BuffType_Mining_Name(),
            "Luck" => I18n.BuffType_Luck_Name(),
            "Foraging" => I18n.BuffType_Foraging_Name(),
            "MaxStamina" => I18n.BuffType_MaxStamina_Name(),
            "MagneticRadius" => I18n.BuffType_MagneticRadius_Name(),
            "Speed" => I18n.BuffType_Speed_Name(),
            "Defense" => I18n.BuffType_Defense_Name(),
            "Attack" => I18n.BuffType_Attack_Name(),
            _ => I18n.BuffType_None_Name()
        };
    }
}