using Common;
using HarmonyLib;
using HelpWanted.Framework;
using HelpWanted.Framework.Patches;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace HelpWanted;

internal partial class ModEntry : Mod
{
    public static IMonitor SMonitor;
    public static IModHelper SHelper;
    private QuestManager questManager;

    public static ModConfig Config = new();
    private const string PadTexturePath = "aedenthorn.HelpWanted/Pad";
    private const string PinTexturePath = "aedenthorn.HelpWanted/Pin";

    private static readonly Random Random = new();
    public static readonly List<QuestData> QuestList = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Config = helper.ReadConfig<ModConfig>();
        SMonitor = Monitor;
        SHelper = helper;
        questManager = new QuestManager(Config, Monitor);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        // 注册Harmony补丁
        HarmonyPatch();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;
        if (Game1.stats.DaysPlayed <= 1 && !Config.QuestFirstDay) return;
        if (Utility.isFestivalDay() && !Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season) && !Config.QuestFestival) return;
        // if (Random.NextDouble() >= Config.DailyQuestChance) return;
        
        Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        questManager.InitQuestList(QuestList);
        Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
    }

    private void HarmonyPatch()
    {
        var harmony = new Harmony("aedenthorn.HelpWanted");
        harmony.Patch(
            AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
            new HarmonyMethod(typeof(BillboardPatch), nameof(BillboardPatch.DrawPrefix))
        );
        harmony.Patch(
            AccessTools.Method(typeof(Billboard), nameof(Billboard.receiveLeftClick)),
            postfix: new HarmonyMethod(typeof(BillboardPatch), nameof(BillboardPatch.ReceiveLeftClickPostfix))
        );
        harmony.Patch(AccessTools.Method(typeof(Utility), nameof(Utility.getRandomItemFromSeason), new[] { typeof(Season), typeof(bool), typeof(Random) }),
            transpiler: new HarmonyMethod(typeof(UtilityPatch), nameof(UtilityPatch.GetRandomItemFromSeasonTranspiler))
        );
        harmony.Patch(
            AccessTools.Method(typeof(ItemDeliveryQuest), "loadQuestInfo"),
            transpiler: new HarmonyMethod(typeof(ItemDeliveryQuestPatch), nameof(ItemDeliveryQuestPatch.LoadQuestInfoTranspiler))
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // 获取GMCM提供的API
        var configMenu = SHelper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // 注册配置菜单
        configMenu.Register(
            ModManifest,
            () => Config = SHelper.ReadConfig<ModConfig>(),
            () => SHelper.WriteConfig(Config)
        );

        // 添加ModEnable配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.ModEnabled,
            value => Config.ModEnabled = value,
            I18n.Config_ModEnabled_Name,
            I18n.Config_ModEnabled_Tooltip
        );

        // 添加QuestFirstDay配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.QuestFirstDay,
            value => Config.QuestFirstDay = value,
            I18n.Config_QuestFirstDay_Name,
            I18n.Config_QuestFirstDay_Tooltip
        );

        // 添加QuestFestival配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.QuestFestival,
            value => Config.QuestFestival = value,
            I18n.Config_QuestFestival_Name,
            I18n.Config_QuestFestival_Tooltip
        );

        // 添加MustLikeItem配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.MustLikeItem,
            value => Config.MustLikeItem = value,
            I18n.Config_MustLikeItem_Name,
            I18n.Config_MustLikeItem_Tooltip
        );

        // 添加MustLoveItem配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.MustLoveItem,
            value => Config.MustLoveItem = value,
            I18n.Config_MustLoveItem_Name,
            I18n.Config_MustLoveItem_Tooltip
        );

        // 添加AllowArtisanGoods配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.AllowArtisanGoods,
            value => Config.AllowArtisanGoods = value,
            I18n.Config_AllowArtisanGoods_Name,
            I18n.Config_AllowArtisanGoods_Tooltip
        );

        // 添加IgnoreVanillaItemRestriction配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.IgnoreVanillaItemRestriction,
            value => Config.IgnoreVanillaItemRestriction = value,
            I18n.Config_IgnoreVanillaItemRestriction_Name,
            I18n.Config_IgnoreVanillaItemRestriction_Tooltip
        );

        // 添加OneQuestPerVillager配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.OneQuestPerVillager,
            value => Config.OneQuestPerVillager = value,
            I18n.Config_OneQuestPerVillager_Name,
            I18n.Config_OneQuestPerVillager_Tooltip
        );

        // 添加AvoidMaxHearts配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.AvoidMaxHearts,
            value => Config.AvoidMaxHearts = value,
            I18n.Config_AvoidMaxHearts_Name,
            I18n.Config_AvoidMaxHearts_Tooltip
        );

        // 添加MaxPrice配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.MaxPrice,
            value => Config.MaxPrice = value,
            I18n.Config_MaxPrice_Name,
            I18n.Config_MaxPrice_Tooltip
        );

        // 添加QuestDays配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.QuestDays,
            value => Config.QuestDays = value,
            I18n.Config_QuestDays_Name,
            I18n.Config_QuestDays_Tooltip
        );

        // 添加MaxQuests配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.MaxQuests,
            value => Config.MaxQuests = value,
            I18n.Config_MaxQuests_Name,
            I18n.Config_MaxQuests_Tooltip
        );

        // 添加NoteScale配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.NoteScale,
            value => Config.NoteScale = value,
            I18n.Config_NoteScale_Name,
            I18n.Config_NoteScale_Tooltip
        );

        // 添加XOverlapBoundary配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.XOverlapBoundary,
            value => Config.XOverlapBoundary = value,
            I18n.Config_XOverlapBoundary_Name,
            I18n.Config_XOverlapBoundary_Tooltip,
            0,
            1,
            0.05f
        );

        // 添加YOverlapBoundary配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.YOverlapBoundary,
            value => Config.YOverlapBoundary = value,
            I18n.Config_YOverlapBoundary_Name,
            I18n.Config_YOverlapBoundary_Tooltip,
            0,
            1,
            0.05f
        );

        // 添加RandomColorMin配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.RandomColorMin,
            value => Config.RandomColorMin = value,
            I18n.Config_RandomColorMin_Name,
            I18n.Config_RandomColorMin_Tooltip,
            0,
            255
        );

        // 添加RandomColorMax配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.RandomColorMax,
            value => Config.RandomColorMax = value,
            I18n.Config_RandomColorMax_Name,
            I18n.Config_RandomColorMax_Tooltip,
            0,
            255
        );

        // 添加PortraitScale配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitScale,
            value => Config.PortraitScale = value,
            I18n.Config_PortraitScale_Name,
            I18n.Config_PortraitScale_Tooltip
        );

        // 添加PortraitOffsetX配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitOffsetX,
            value => Config.PortraitOffsetX = value,
            I18n.Config_PortraitOffsetX_Name,
            I18n.Config_PortraitOffsetX_Tooltip
        );

        // 添加PortraitOffsetY配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitOffsetY,
            value => Config.PortraitOffsetY = value,
            I18n.Config_PortraitOffsetY_Name,
            I18n.Config_PortraitOffsetY_Tooltip
        );

        // 添加PortraitTintR配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintR,
            value => Config.PortraitTintR = value,
            I18n.Config_PortraitTintR_Name,
            I18n.Config_PortraitTintR_Tooltip,
            0,
            255
        );

        // 添加PortraitTintG配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintG,
            value => Config.PortraitTintG = value,
            I18n.Config_PortraitTintG_Name,
            I18n.Config_PortraitTintG_Tooltip,
            0,
            255
        );

        // 添加PortraitTintB配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintB,
            value => Config.PortraitTintB = value,
            I18n.Config_PortraitTintB_Name,
            I18n.Config_PortraitTintB_Tooltip,
            0,
            255
        );

        // 添加PortraitTintA配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintA,
            value => Config.PortraitTintA = value,
            I18n.Config_PortraitTintA_Name,
            I18n.Config_PortraitTintA_Tooltip,
            0,
            255
        );

        // 添加ResourceCollectionWeight配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.ResourceCollectionWeight,
            value => Config.ResourceCollectionWeight = value,
            I18n.Config_ResourceCollectionWeight_Name,
            I18n.Config_ResourceCollectionWeight_Tooltip
        );

        // 添加SlayMonstersWeight配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.SlayMonstersWeight,
            value => Config.SlayMonstersWeight = value,
            I18n.Config_SlayMonstersWeight_Name,
            I18n.Config_SlayMonstersWeight_Tooltip
        );

        // 添加FishingWeight配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.FishingWeight,
            value => Config.FishingWeight = value,
            I18n.Config_FishingWeight_Name,
            I18n.Config_FishingWeight_Tooltip
        );

        // 添加ItemDeliveryWeight配置选项
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.ItemDeliveryWeight,
            value => Config.ItemDeliveryWeight = value,
            I18n.Config_ItemDeliveryWeight_Name,
            I18n.Config_ItemDeliveryWeight_Tooltip
        );
    }
}