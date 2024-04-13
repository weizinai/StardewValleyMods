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
    }
    
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        lazyModManager?.Update();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

        if (configMenu is null)
            return;

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
            "Other",
            I18n.Config_OtherPage_Name
        );

        configMenu.AddPage(
            ModManifest,
            "Farming",
            I18n.Config_FarmingPage_Name
        );

        #region 自动耕地

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

        #endregion

        #region 自动浇水

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

        #endregion

        #region 自动补充水壶

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

        #endregion

        #region 自动播种

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

        #endregion

        #region 自动施肥

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

        #endregion

        #region 自动收获作物

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

        #endregion

        configMenu.AddPage(
            ModManifest,
            "Other",
            I18n.Config_OtherPage_Name
        );

        #region 自动挖掘远古斑点

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