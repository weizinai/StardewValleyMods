using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;
using weizinai.StardewValleyMod.LazyMod.Handler;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Hud;

namespace weizinai.StardewValleyMod.LazyMod;

internal class ModEntry : Mod
{
    private ConfigService<ModConfig> configService = null!;

    private int cooldownTimer;
    private bool modEnable = true;

    private IAutomationHandler[] handlers = null!;
    private IAutomationHandlerWithDayChanged[] dayChangedHandlers = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);

        this.configService = new ConfigService<ModConfig>(this, this.UpdateConfig);
        this.configService.RegisterMenu(this.BuildConfigMenu);

        // 注册事件
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.DayEnding += this.OnDayEnding;

        helper.Events.Player.InventoryChanged += this.OnInventoryChanged;

        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;

        this.UpdateConfig();
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单（主页 + 八个功能页），由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            // 主页：打开/开关按键与冷却，以及八个功能页链接
            .AddKeybindListOption(config => config.OpenConfigMenuKeybind, I18n.Config_OpenConfigMenuKeybind_Name)
            .AddKeybindListOption(
                config => config.ToggleModStateKeybind,
                I18n.Config_ToggleModStateKeybind_Name,
                I18n.Config_ToggleModStateKeybind_Tooltip
            )
            .AddNumberOption(config => config.Cooldown, I18n.Config_Cooldown_Name, I18n.Config_Cooldown_Tooltip, 0, 60, 5)
            .AddPageLink("Farming", I18n.Config_FarmingPage_Name)
            .AddPageLink("Animal", I18n.Config_AnimalPage_Name)
            .AddPageLink("Mining", I18n.Config_MiningPage_Name)
            .AddPageLink("Foraging", I18n.Config_ForagingPage_Name)
            .AddPageLink("Fishing", I18n.Config_FishingPage_Name)
            .AddPageLink("Food", I18n.Config_FoodPage_Name)
            .AddPageLink("Other", I18n.Config_OtherPage_Name);

        AddFarmingPage(menu);
        AddAnimalPage(menu);
        AddMiningPage(menu);
        AddForagingPage(menu);
        AddFishingPage(menu);
        AddFoodPage(menu);
        AddOtherPage(menu);
        AddTreeSettingsPage(menu);
    }

    /// <summary>声明耕种页：各自动功能分区（体力工具形状）与零散开关。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddFarmingPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddPage("Farming", I18n.Config_FarmingPage_Name)
            // 自动耕地
            .AddSection(
                config => config.AutoTillDirt,
                I18n.Config_AutoTillDirt_Name,
                section => ConfigureStaminaToolAutomationSection(section, 0),
                I18n.Config_AutoTillDirt_Tooltip
            )
            // 自动清理耕地
            .AddSection(
                config => config.AutoClearTilledDirt,
                I18n.Config_AutoClearTilledDirt_Name,
                section => ConfigureStaminaToolAutomationSection(section, 0),
                I18n.Config_AutoClearTilledDirt_Tooltip
            )
            // 自动浇水
            .AddSection(
                config => config.AutoWaterDirt,
                I18n.Config_AutoWaterDirt_Name,
                section => ConfigureStaminaToolAutomationSection(section, 0),
                I18n.Config_AutoWaterDirt_Tooltip
            )
            .AddBoolOption(config => config.WaterOnlyWhenCrop, I18n.Config_WaterOnlyWhenCrop_Name, I18n.Config_WaterOnlyWhenCrop_Tooltip)
            // 自动补充水壶
            .AddSection(
                config => config.AutoRefillWateringCan,
                I18n.Config_AutoRefillWateringCan_Name,
                section => ConfigureToolAutomationSection(section, 1),
                I18n.Config_AutoRefillWateringCan_Tooltip
            )
            // 自动播种
            .AddSection(
                config => config.AutoSeed,
                I18n.Config_AutoSeed_Name,
                section => ConfigureBaseAutomationSection(section, 0),
                I18n.Config_AutoSeed_Tooltip
            )
            // 自动施肥
            .AddSection(
                config => config.AutoFertilize,
                I18n.Config_AutoFertilize_Name,
                section => ConfigureBaseAutomationSection(section, 0),
                I18n.Config_AutoFertilize_Tooltip
            )
            // 自动收获作物
            .AddSection(config => config.AutoHarvestCrop, I18n.Config_AutoHarvestCrop_Name, section => ConfigureBaseAutomationSection(section, 0))
            .AddBoolOption(config => config.AutoHarvestFlower, I18n.Config_AutoHarvestFlower_Name)
            // 自动摇晃果树
            .AddSection(config => config.AutoShakeFruitTree, I18n.Config_AutoShakeFruitTree_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动清理枯萎作物
            .AddSection(
                config => config.AutoClearDeadCrop,
                I18n.Config_AutoClearDeadCrop_Name,
                section => ConfigureToolAutomationSection(section, 0),
                I18n.Config_AutoClearDeadCrop_Tooltip
            );
    }

    /// <summary>声明动物页：各自动功能分区（基础/体力工具形状）与动物门开关分区。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddAnimalPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddPage("Animal", I18n.Config_AnimalPage_Name)
            // 自动抚摸动物
            .AddSection(config => config.AutoPetAnimal, I18n.Config_AutoPetAnimal_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动抚摸宠物
            .AddSection(config => config.AutoPetPet, I18n.Config_AutoPetPet_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动挤奶
            .AddSection(
                config => config.AutoMilkAnimal,
                I18n.Config_AutoMilkAnimal_Name,
                section => ConfigureStaminaToolAutomationSection(section, 1),
                I18n.Config_AutoMilkAnimal_Tooltip
            )
            // 自动剪毛
            .AddSection(
                config => config.AutoShearsAnimal,
                I18n.Config_AutoShearsAnimal_Name,
                section => ConfigureStaminaToolAutomationSection(section, 1),
                I18n.Config_AutoShearsAnimal_Tooltip
            )
            // 自动喂食动物饼干
            .AddSection(
                config => config.AutoFeedAnimalCracker,
                I18n.Config_AutoFeedAnimalCracker_Name,
                section => ConfigureBaseAutomationSection(section, 1),
                I18n.Config_AutoFeedAnimalCracker_Tooltip
            )
            // 自动打开动物门
            .AddBoolSection(
                config => config.AutoOpenAnimalDoor,
                I18n.Config_AutoOpenAnimalDoor_Name,
                I18n.Config_AutoOpenAnimalDoor_Tooltip
            )
            // 自动打开栅栏门
            .AddSection(config => config.AutoOpenFenceGate, I18n.Config_AutoOpenFenceGate_Name, section => ConfigureBaseAutomationSection(section, 1));
    }

    /// <summary>声明采矿页：自动清理石头分区与清除位置/石头类型开关、其余自动功能分区。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddMiningPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddPage("Mining", I18n.Config_MiningPage_Name)
            // 自动清理石头
            .AddSection(
                config => config.AutoClearStone,
                I18n.Config_AutoClearStone_Name,
                section => ConfigureStaminaToolAutomationSection(section, 1),
                I18n.Config_AutoClearStone_Tooltip
            )
            .AddBoolOption(config => config.ClearStoneOnMineShaft, I18n.Config_ClearStoneOnMineShaft_Name, I18n.Config_ClearStoneOnMineShaft_Tooltip)
            .AddBoolOption(config => config.ClearStoneOnVolcano, I18n.Config_ClearStoneOnVolcano_Name, I18n.Config_ClearStoneOnVolcano_Tooltip)
            .AddBoolOption(config => config.ClearFarmStone, I18n.Config_ClearFarmStone_Name, I18n.Config_ClearFarmStone_Tooltip)
            .AddBoolOption(config => config.ClearOtherStone, I18n.Config_ClearOtherStone_Name, I18n.Config_ClearOtherStone_Tooltip)
            .AddBoolOption(config => config.ClearIslandStone, I18n.Config_ClearIslandStone_Name, I18n.Config_ClearIslandStone_Tooltip)
            .AddBoolOption(config => config.ClearOreStone, I18n.Config_ClearOreStone_Name)
            .AddBoolOption(config => config.ClearGemStone, I18n.Config_ClearGemStone_Name)
            .AddBoolOption(config => config.ClearGeodeStone, I18n.Config_ClearGeodeStone_Name)
            .AddBoolOption(config => config.ClearCalicoEggStone, I18n.Config_ClearCalicoEggStone_Name)
            .AddBoolOption(config => config.ClearBoulder, I18n.Config_ClearBoulder_Name)
            .AddBoolOption(config => config.ClearMeteorite, I18n.Config_ClearMeteorite_Name)
            // 自动收集煤炭
            .AddSection(
                config => config.AutoCollectCoal,
                I18n.Config_AutoCollectCoal_Name,
                section => ConfigureBaseAutomationSection(section, 1),
                I18n.Config_AutoCollectCoal_Tooltip
            )
            // 自动破坏容器
            .AddSection(
                config => config.AutoBreakContainer,
                I18n.Config_AutoBreakContainer_Name,
                section => ConfigureToolAutomationSection(section, 1),
                I18n.Config_AutoBreakContainer_Tooltip
            )
            // 自动打开宝藏
            .AddSection(config => config.AutoOpenTreasure, I18n.Config_AutoOpenTreasure_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动清理水晶
            .AddSection(config => config.AutoClearCrystal, I18n.Config_AutoClearCrystal_Name, section => ConfigureToolAutomationSection(section, 1))
            // 自动冷却岩浆
            .AddSection(
                config => config.AutoCoolLava,
                I18n.Config_AutoCoolLava_Name,
                section => ConfigureStaminaToolAutomationSection(section, 1),
                I18n.Config_AutoCoolLava_Tooltip
            );
    }

    /// <summary>声明采集页：自动觅食/砍树等分区，中间嵌入指向树木设置页的链接。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddForagingPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddPage("Foraging", I18n.Config_ForagingPage_Name)
            // 自动觅食
            .AddSection(config => config.AutoForage, I18n.Config_AutoForage_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动砍树
            .AddSection(
                config => config.AutoChopTree,
                I18n.Config_AutoChopTree_Name,
                section => ConfigureStaminaToolAutomationSection(section, 1),
                I18n.Config_AutoChopTree_Tooltip
            )
            .AddPageLink("TreeSettings", I18n.Config_TreeSettingsPage_Name)
            // 自动收获姜
            .AddSection(
                config => config.AutoHarvestGinger,
                I18n.Config_AutoHarvestGinger_Name,
                section => ConfigureStaminaToolAutomationSection(section, 0),
                I18n.Config_AutoHarvestGinger_Tooltip
            )
            // 自动摇树
            .AddSection(config => config.AutoShakeTree, I18n.Config_AutoShakeTree_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动收获苔藓
            .AddSection(
                config => config.AutoHarvestMoss,
                I18n.Config_AutoHarvestMoss_Name,
                section => ConfigureToolAutomationSection(section, 1),
                I18n.Config_AutoHarvestMoss_Tooltip
            )
            // 自动放置采集器
            .AddSection(
                config => config.AutoPlaceTapper,
                I18n.Config_AutoPlaceTapper_Name,
                section => ConfigureBaseAutomationSection(section, 1),
                I18n.Config_AutoPlaceTapper_Tooltip
            )
            // 自动在树上浇醋
            .AddSection(
                config => config.AutoPlaceVinegar,
                I18n.Config_AutoPlaceVinegar_Name,
                section => ConfigureBaseAutomationSection(section, 1),
                I18n.Config_AutoPlaceVinegar_Tooltip
            )
            // 自动清理木头
            .AddSection(
                config => config.AutoClearWood,
                I18n.Config_AutoClearWood_Name,
                section => ConfigureStaminaToolAutomationSection(section, 1),
                I18n.Config_AutoClearWood_Tooltip
            )
            .AddBoolOption(config => config.ClearStump, I18n.Config_ClearStump_Name)
            .AddBoolOption(config => config.ClearHollowLog, I18n.Config_ClearHollowLog_Name);
    }

    /// <summary>声明钓鱼页：两个宝箱开关与蟹笼相关的自动功能分区。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddFishingPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddPage("Fishing", I18n.Config_FishingPage_Name)
            // 自动抓取宝箱物品
            .AddBoolOption(config => config.AutoGrabTreasureItem, I18n.Config_AutoGrabTreasureItem_Name)
            // 自动退出宝箱菜单
            .AddBoolOption(config => config.AutoExitTreasureMenu, I18n.Config_AutoExitTreasureMenu_Name)
            // 自动放置蟹笼
            .AddSection(
                config => config.AutoPlaceCarbPot,
                I18n.Config_AutoPlaceCarbPot_Name,
                section => ConfigureBaseAutomationSection(section, 1),
                I18n.Config_AutoPlaceCarbPot_Tooltip
            )
            // 自动添加蟹笼鱼饵
            .AddSection(
                config => config.AutoAddBaitForCarbPot,
                I18n.Config_AutoAddBaitForCarbPot_Name,
                section => ConfigureBaseAutomationSection(section, 1),
                I18n.Config_AutoAddBaitForCarbPot_Tooltip
            )
            // 自动收获蟹笼
            .AddSection(config => config.AutoHarvestCarbPot, I18n.Config_AutoHarvestCarbPot_Name, section => ConfigureBaseAutomationSection(section, 1));
    }

    /// <summary>声明食物页：体力/生命值/增益食物/增益饮料四组分区，含枚举类型的 buff 维持选项。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddFoodPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddPage("Food", I18n.Config_FoodPage_Name)
            // 自动吃食物-体力
            .AddBoolSection(config => config.AutoEatFoodForStamina, I18n.Config_AutoEatFoodForStamina_Name)
            .AddNumberOption(config => config.AutoEatFoodStaminaRate, I18n.Config_AutoEatFoodStaminaRate_Name, null, 0.05f, 0.95f, 0.05f)
            .AddBoolOption(
                config => config.IntelligentFoodSelectionForStamina,
                I18n.Config_IntelligentFoodSelectionForStamina_Name,
                I18n.Config_IntelligentFoodSelectionForStamina_Tooltip
            )
            .AddNumberOption(
                config => config.RedundantStaminaFoodCount,
                I18n.Config_RedundantFoodCount_Name,
                I18n.Config_RedundantFoodCount_Tooltip,
                0,
                50,
                5
            )
            // 自动吃食物-生命值
            .AddBoolSection(config => config.AutoEatFoodForHealth, I18n.Config_AutoEatFoodForHealth_Name)
            .AddNumberOption(config => config.AutoEatFoodHealthRate, I18n.Config_AutoEatFoodHealthRate_Name, null, 0.05f, 0.95f, 0.05f)
            .AddBoolOption(
                config => config.IntelligentFoodSelectionForHealth,
                I18n.Config_IntelligentFoodSelectionForHealth_Name,
                I18n.Config_IntelligentFoodSelectionForHealth_Tooltip
            )
            .AddNumberOption(
                config => config.RedundantHealthFoodCount,
                I18n.Config_RedundantFoodCount_Name,
                I18n.Config_RedundantFoodCount_Tooltip,
                0,
                50,
                5
            )
            // 自动吃增益食物
            .AddBoolSection(config => config.AutoEatBuffFood, I18n.Config_AutoEatBuffFood_Name, I18n.Config_AutoEatBuffFood_Tooltip)
            .AddEnumOption(
                config => config.FoodBuffMaintain1,
                I18n.Config_FoodBuffMaintain1_Name,
                I18n.Config_FoodBuffMaintain_Tooltip,
                formatValue: FormatBuffTypeName
            )
            .AddEnumOption(
                config => config.FoodBuffMaintain2,
                I18n.Config_FoodBuffMaintain2_Name,
                I18n.Config_FoodBuffMaintain_Tooltip,
                formatValue: FormatBuffTypeName
            )
            // 自动喝增益饮料
            .AddBoolSection(config => config.AutoDrinkBuffDrink, I18n.Config_AutoDrinkBuffDrink_Name, I18n.Config_AutoDrinkBuffDrink_Tooltip)
            .AddEnumOption(
                config => config.DrinkBuffMaintain1,
                I18n.Config_DrinkBuffMaintain1_Name,
                I18n.Config_DrinkBuffMaintain_ToolTip,
                formatValue: FormatBuffTypeName
            )
            .AddEnumOption(
                config => config.DrinkBuffMaintain2,
                I18n.Config_DrinkBuffMaintain2_Name,
                I18n.Config_DrinkBuffMaintain_ToolTip,
                formatValue: FormatBuffTypeName
            );
    }

    /// <summary>声明其他页：磁力半径、自动清理杂草等其余自动功能分区。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddOtherPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddPage("Other", I18n.Config_OtherPage_Name)
            // 磁力范围增加
            .AddSectionTitle(I18n.Config_MagneticRadiusIncrease_Name)
            .AddNumberOption(config => config.MagneticRadiusIncrease, I18n.Config_MagneticRadiusIncrease_Name, null, 0, 10)
            // 自动清理杂草
            .AddSection(
                config => config.AutoClearWeeds,
                I18n.Config_AutoClearWeeds_Name,
                section => ConfigureToolAutomationSection(section, 1),
                I18n.Config_AutoClearWeeds_Tooltip
            )
            .AddBoolOption(config => config.ClearLargeWeeds, I18n.Config_ClearLargeWeeds_Name)
            // 自动挖掘斑点
            .AddSection(
                config => config.AutoDigSpots,
                I18n.Config_AutoDigSpots_Name,
                section => ConfigureStaminaToolAutomationSection(section, 0),
                I18n.Config_AutoDigSpots_Tooltip
            )
            // 自动收获机器
            .AddSection(config => config.AutoHarvestMachine, I18n.Config_AutoHarvestMachine_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动触发机器
            .AddSection(config => config.AutoTriggerMachine, I18n.Config_AutoTriggerMachine_Name, section => ConfigureBaseAutomationSection(section, 1))
            // 自动使用仙尘
            .AddSection(
                config => config.AutoUseFairyDust,
                I18n.Config_AutoUseFairyDust_Name,
                section => ConfigureBaseAutomationSection(section, 1),
                I18n.Config_AutoUseFairyDust_Tooltip
            )
            // 自动翻垃圾桶
            .AddSection(config => config.AutoGarbageCan, I18n.Config_AutoGarbageCan_Name, section => ConfigureBaseAutomationSection(section, 1))
            .AddBoolOption(config => config.StopGarbageCanNearVillager, I18n.Config_StopGarbageCanNearVillager_Name)
            // 自动放置地板
            .AddSection(
                config => config.AutoPlaceFloor,
                I18n.Config_AutoPlaceFloor_Name,
                section => ConfigureBaseAutomationSection(section, 0),
                I18n.Config_AutoPlaceFloor_Tooltip
            );
    }

    /// <summary>
    /// 声明树木设置页。成长阶段存于 <see cref="Dictionary{TKey,TValue}" />，其下标在表达式树中是 get_Item/set_Item 方法调用，
    /// 成员绑定不接受（逃生舱只接受属性/字段链），故整页经逃生舱按原始 GMCM API 注册。
    /// </summary>
    /// <param name="menu">配置菜单描述器。</param>
    private static void AddTreeSettingsPage(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu.AddPage("TreeSettings", I18n.Config_TreeSettingsPage_Name);
        menu.AddCustomSection(AddTreeSettingsContent);
    }

    /// <summary>逃生舱内容：注册八棵树各自的成长阶段开关（每棵一个分区标题 + 各成长阶段一个开关）。</summary>
    /// <param name="configMenu">原始 GMCM 集成对象。</param>
    private static void AddTreeSettingsContent(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        // 橡树
        AddTreeGrowthStageSection(configMenu, config => config.ChopOakTree, I18n.Config_OakTreeTitle_Name,
            (0, I18n.Config_ChopSeedStageOakTree_Name),
            (1, I18n.Config_ChopSproutStageOakTree_Name),
            (2, I18n.Config_ChopSaplingStageOakTree_Name),
            (3, I18n.Config_ChopBushStageOakTree_Name),
            (4, I18n.Config_ChopSmallTreeStageOakTree_Name),
            (5, I18n.Config_ChopOakTree_Name),
            (-1, I18n.Config_ChopStumpStageOakTree_Name));
        // 枫树
        AddTreeGrowthStageSection(configMenu, config => config.ChopMapleTree, I18n.Config_MapleTreeTitle_Name,
            (0, I18n.Config_ChopSeedStageMapleTree_Name),
            (1, I18n.Config_ChopSproutStageMapleTree_Name),
            (2, I18n.Config_ChopSaplingStageMapleTree_Name),
            (3, I18n.Config_ChopBushStageMapleTree_Name),
            (4, I18n.Config_ChopSmallTreeStageMapleTree_Name),
            (5, I18n.Config_ChopMapleTree_Name),
            (-1, I18n.Config_ChopStumpStageMapleTree_Name));
        // 松树
        AddTreeGrowthStageSection(configMenu, config => config.ChopPineTree, I18n.Config_PineTreeTitle_Name,
            (0, I18n.Config_ChopSeedStagePineTree_Name),
            (1, I18n.Config_ChopSproutStagePineTree_Name),
            (2, I18n.Config_ChopSaplingStagePineTree_Name),
            (3, I18n.Config_ChopBushStagePineTree_Name),
            (4, I18n.Config_ChopSmallTreeStagePineTree_Name),
            (5, I18n.Config_ChopPineTree_Name),
            (-1, I18n.Config_ChopStumpStagePineTree_Name));
        // 桃花心木树
        AddTreeGrowthStageSection(configMenu, config => config.ChopMahoganyTree, I18n.Config_MahoganyTreeTitle_Name,
            (0, I18n.Config_ChopSeedStageMahoganyTree_Name),
            (1, I18n.Config_ChopSproutStageMahoganyTree_Name),
            (2, I18n.Config_ChopSaplingStageMahoganyTree_Name),
            (3, I18n.Config_ChopBushStageMahoganyTree_Name),
            (4, I18n.Config_ChopSmallTreeStageMahoganyTree_Name),
            (5, I18n.Config_ChopMahoganyTree_Name),
            (-1, I18n.Config_ChopStumpStageMahoganyTree_Name));
        // 棕榈树
        AddTreeGrowthStageSection(configMenu, config => config.ChopPalmTree, I18n.Config_PalmTreeTitle_Name,
            (2, I18n.Config_ChopSaplingStagePalmTree_Name),
            (3, I18n.Config_ChopBushStagePalmTree_Name),
            (4, I18n.Config_ChopSmallTreeStagePalmTree_Name),
            (5, I18n.Config_ChopPalmTree_Name),
            (-1, I18n.Config_ChopStumpStagePalmTree_Name));
        // 蘑菇树
        AddTreeGrowthStageSection(configMenu, config => config.ChopMushroomTree, I18n.Config_MushroomTreeTitle_Name,
            (0, I18n.Config_ChopSeedStageMushroomTree_Name),
            (1, I18n.Config_ChopSproutStageMushroomTree_Name),
            (2, I18n.Config_ChopSaplingStageMushroomTree_Name),
            (3, I18n.Config_ChopBushStageMushroomTree_Name),
            (4, I18n.Config_ChopSmallTreeStageMushroomTree_Name),
            (5, I18n.Config_ChopMushroomTree_Name),
            (-1, I18n.Config_ChopStumpStageMushroomTree_Name));
        // 苔雨树
        AddTreeGrowthStageSection(configMenu, config => config.ChopGreenRainTree, I18n.Config_GreenRainTreeTitle_Name,
            (0, I18n.Config_ChopSeedStageGreenRainTree_Name),
            (1, I18n.Config_ChopSproutStageGreenRainTree_Name),
            (2, I18n.Config_ChopSaplingStageGreenRainTree_Name),
            (3, I18n.Config_ChopBushStageGreenRainTree_Name),
            (4, I18n.Config_ChopSmallTreeStageGreenRainTree_Name),
            (5, I18n.Config_ChopGreenRainTree_Name),
            (-1, I18n.Config_ChopStumpStageGreenRainTree_Name));
        // 神秘树
        AddTreeGrowthStageSection(configMenu, config => config.ChopMysticTree, I18n.Config_MysticTreeTitle_Name,
            (0, I18n.Config_ChopSeedStageMysticTree_Name),
            (1, I18n.Config_ChopSproutStageMysticTree_Name),
            (2, I18n.Config_ChopSaplingStageMysticTree_Name),
            (3, I18n.Config_ChopBushStageMysticTree_Name),
            (4, I18n.Config_ChopSmallTreeStageMysticTree_Name),
            (5, I18n.Config_ChopMysticTree_Name),
            (-1, I18n.Config_ChopStumpStageMysticTree_Name));
    }

    /// <summary>逃生舱内容助手：把一棵树的成长阶段字典渲染成一个分区标题 + 每个阶段一个开关。</summary>
    /// <param name="configMenu">原始 GMCM 集成对象。</param>
    /// <param name="getTree">取该树成长阶段字典的委托。</param>
    /// <param name="title">分区标题。</param>
    /// <param name="stages">成长阶段键与标签的有序列表。</param>
    private static void AddTreeGrowthStageSection(
        GenericModConfigMenuIntegration<ModConfig> configMenu,
        Func<ModConfig, Dictionary<int, bool>> getTree,
        Func<string> title,
        params (int Stage, Func<string> Label)[] stages
    )
    {
        configMenu.AddSectionTitle(title);

        foreach (var (stage, label) in stages)
        {
            configMenu.AddBoolOption(
                config => getTree(config)[stage],
                (config, value) => getTree(config)[stage] = value,
                label
            );
        }
    }

    /// <summary>把 buff 类型本地化成菜单显示的文本。</summary>
    /// <param name="buffType">buff 类型。</param>
    /// <returns>本地化名称。</returns>
    private static string FormatBuffTypeName(BuffType buffType)
    {
        return buffType switch
        {
            BuffType.Combat => I18n.BuffType_Combat_Name(),
            BuffType.Farming => I18n.BuffType_Farming_Name(),
            BuffType.Fishing => I18n.BuffType_Fishing_Name(),
            BuffType.Mining => I18n.BuffType_Mining_Name(),
            BuffType.Luck => I18n.BuffType_Luck_Name(),
            BuffType.Foraging => I18n.BuffType_Foraging_Name(),
            BuffType.MaxStamina => I18n.BuffType_MaxStamina_Name(),
            BuffType.MagneticRadius => I18n.BuffType_MagneticRadius_Name(),
            BuffType.Speed => I18n.BuffType_Speed_Name(),
            BuffType.Defense => I18n.BuffType_Defense_Name(),
            BuffType.Attack => I18n.BuffType_Attack_Name(),
            _ => I18n.BuffType_None_Name()
        };
    }

    /// <summary>渲染“基础自动化”子配置分区：开关 + 范围两个选项。</summary>
    /// <param name="section">子配置分区构建器。</param>
    /// <param name="minRange">范围可选最小值。</param>
    private static void ConfigureBaseAutomationSection(ConfigMenuSection<ModConfig, BaseAutomationConfig> section, int minRange)
    {
        section
            .AddBoolOption(config => config.IsEnable, I18n.Config_Enable_Name)
            .AddNumberOption(config => config.Range, I18n.Config_Range_Name, null, minRange, 5);
    }

    /// <summary>渲染“工具自动化”子配置分区：基础形状 + 从背包找工具开关。</summary>
    /// <param name="section">子配置分区构建器。</param>
    /// <param name="minRange">范围可选最小值。</param>
    private static void ConfigureToolAutomationSection(ConfigMenuSection<ModConfig, ToolAutomationConfig> section, int minRange)
    {
        section
            .AddBoolOption(config => config.IsEnable, I18n.Config_Enable_Name)
            .AddNumberOption(config => config.Range, I18n.Config_Range_Name, null, minRange, 5)
            .AddBoolOption(config => config.FindToolFromInventory, I18n.Config_FindToolFromInventory_Name, I18n.Config_FindToolFromInventory_Tooltip);
    }

    /// <summary>渲染“体力工具自动化”子配置分区：基础形状 + 体力阈值 + 从背包找工具开关。</summary>
    /// <param name="section">子配置分区构建器。</param>
    /// <param name="minRange">范围可选最小值。</param>
    private static void ConfigureStaminaToolAutomationSection(ConfigMenuSection<ModConfig, StaminaToolAutomationConfig> section, int minRange)
    {
        section
            .AddBoolOption(config => config.IsEnable, I18n.Config_Enable_Name)
            .AddNumberOption(config => config.Range, I18n.Config_Range_Name, null, minRange, 5)
            .AddNumberOption(config => config.StopStamina, I18n.Config_StopStamina_Name)
            .AddBoolOption(config => config.FindToolFromInventory, I18n.Config_FindToolFromInventory_Name, I18n.Config_FindToolFromInventory_Tooltip);
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
        if (!this.modEnable)
        {
            return;
        }

        if (!this.UpdateCooldown())
        {
            return;
        }

        var player = Game1.player;
        var location = Game1.currentLocation;

        if (player is null || location is null)
        {
            return;
        }

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
        if (!Context.IsPlayerFree)
        {
            return;
        }

        if (ModConfig.Instance.ToggleModStateKeybind.JustPressed())
        {
            this.modEnable = !this.modEnable;
            HudLogger.NoIconHUDMessage(this.modEnable ? I18n.UI_ModState_Enable() : I18n.UI_ModState_Disable());
        }

        if (ModConfig.Instance.OpenConfigMenuKeybind.JustPressed())
        {
            this.configService.OpenMenu();
        }
    }

    private bool UpdateCooldown()
    {
        this.cooldownTimer++;

        if (this.cooldownTimer < ModConfig.Instance.Cooldown)
        {
            return false;
        }

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
        var config = ModConfig.Instance;

        // Farming
        if (config.AutoTillDirt.IsEnable) yield return new TillDirtHandler(config);
        if (config.AutoClearTilledDirt.IsEnable) yield return new ClearTilledDirtHandler(config);
        if (config.AutoWaterDirt.IsEnable) yield return new WaterDirtHandler(config);
        if (config.AutoRefillWateringCan.IsEnable) yield return new RefillWateringCanHandler(config);
        if (config.AutoSeed.IsEnable) yield return new SeedHandler(config);
        if (config.AutoFertilize.IsEnable) yield return new FertilizeHandler(config);
        if (config.AutoHarvestCrop.IsEnable) yield return new HarvestCropHandler(config);
        if (config.AutoShakeFruitTree.IsEnable) yield return new ShakeFruitTreeHandler(config);
        if (config.AutoClearDeadCrop.IsEnable) yield return new ClearDeadCropHandler(config);

        // Animal
        if (config.AutoPetAnimal.IsEnable) yield return new PetAnimalHandler(config);
        if (config.AutoPetPet.IsEnable) yield return new PetPetHandler(config);
        if (config.AutoMilkAnimal.IsEnable) yield return new MilkAnimalHandler(config);
        if (config.AutoShearsAnimal.IsEnable) yield return new ShearsAnimalHandler(config);
        if (config.AutoFeedAnimalCracker.IsEnable) yield return new AnimalCrackerHandler(config);
        if (config.AutoOpenAnimalDoor) yield return new AnimalDoorHandler(config);
        if (config.AutoOpenFenceGate.IsEnable) yield return new FenceGateHandler(config);

        // Mining
        if (config.AutoClearStone.IsEnable) yield return new ClearStoneHandler(config);
        if (config.AutoCollectCoal.IsEnable) yield return new CollectCoalHandler(config);
        if (config.AutoBreakContainer.IsEnable) yield return new BreakContainerHandler(config);
        if (config.AutoOpenTreasure.IsEnable) yield return new OpenTreasureHandler(config);
        if (config.AutoClearCrystal.IsEnable) yield return new ClearCrystalHandler(config);
        if (config.AutoCoolLava.IsEnable) yield return new CoolLavaHandler(config);

        // Foraging
        if (config.AutoForage.IsEnable) yield return new ForageHandler(config);
        if (config.AutoHarvestGinger.IsEnable) yield return new HarvestGingerHandler(config);
        if (config.AutoChopTree.IsEnable) yield return new ChopTreeHandler(config);
        if (config.AutoShakeTree.IsEnable) yield return new ShakeTreeHandler(config);
        if (config.AutoHarvestMoss.IsEnable) yield return new HarvestMossHandler(config);
        if (config.AutoPlaceTapper.IsEnable) yield return new PlaceTapperHandler(config);
        if (config.AutoPlaceVinegar.IsEnable) yield return new PlaceVinegarHandler(config);
        if (config.AutoClearWood.IsEnable) yield return new ClearWoodHandler(config);

        // Fishing
        if (config.AutoGrabTreasureItem) yield return new GrabTreasureItemHandler(config);
        if (config.AutoExitTreasureMenu) yield return new ExitTreasureMenuHandler(config);
        if (config.AutoPlaceCarbPot.IsEnable) yield return new PlaceCrabPotHandler(config);
        if (config.AutoAddBaitForCarbPot.IsEnable) yield return new AddBaitForCrabPotHandler(config);
        if (config.AutoHarvestCarbPot.IsEnable) yield return new HarvestCrabPotHandler(config);

        // Food
        yield return new FoodHandler(config);

        // Other
        yield return new MagneticRadiusHandler(config);

        if (config.AutoClearWeeds.IsEnable) yield return new ClearWeedsHandler(config);
        if (config.AutoDigSpots.IsEnable) yield return new DigSpotHandler(config);
        if (config.AutoHarvestMachine.IsEnable) yield return new HarvestMachineHandler(config);
        if (config.AutoTriggerMachine.IsEnable) yield return new TriggerMachineHandler(config);
        if (config.AutoUseFairyDust.IsEnable) yield return new FairyDustHandler(config);
        if (config.AutoGarbageCan.IsEnable) yield return new GarbageCanHandler(config);
        if (config.AutoPlaceFloor.IsEnable) yield return new PlaceFloorHandler(config);
    }
}
