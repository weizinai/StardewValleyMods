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
    private QuestManager questManager;

    public static ModConfig Config = new();

    private static readonly Random Random = new();
    public static readonly List<QuestData> QuestList = new();
    
    private const string PadTexturePath = "aedenthorn.HelpWanted/Pad";
    private const string PinTexturePath = "aedenthorn.HelpWanted/Pin";

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Config = helper.ReadConfig<ModConfig>();
        SMonitor = Monitor;
        questManager = new QuestManager(Config, Monitor, new AppearanceManager(helper, Config));
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
        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !Config.QuestFestival) return;
        if (Random.NextDouble() >= Config.DailyQuestChance) return;

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
            AccessTools.Method(typeof(ItemDeliveryQuest), nameof(ItemDeliveryQuest.loadQuestInfo)),
            transpiler: new HarmonyMethod(typeof(ItemDeliveryQuestPatch), nameof(ItemDeliveryQuestPatch.LoadQuestInfoTranspiler))
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => Config = new ModConfig(),
            () => Helper.WriteConfig(Config)
        );

        // 一般设置
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_GeneralSettingsTitle_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.QuestFirstDay,
            value => Config.QuestFirstDay = value,
            I18n.Config_QuestFirstDay_Name,
            I18n.Config_QuestFirstDay_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.QuestFestival,
            value => Config.QuestFestival = value,
            I18n.Config_QuestFestival_Name,
            I18n.Config_QuestFestival_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.DailyQuestChance,
            value => Config.DailyQuestChance = value,
            I18n.Config_DailyQuestChance_Name
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.OneQuestPerVillager,
            value => Config.OneQuestPerVillager = value,
            I18n.Config_OneQuestPerVillager_Name,
            I18n.Config_OneQuestPerVillager_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.ExcludeMaxHeartsNPC,
            value => Config.ExcludeMaxHeartsNPC = value,
            I18n.Config_ExcludeMaxHeartsNPC_Name,
            I18n.Config_ExcludeMaxHeartsNPC_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.QuestDays,
            value => Config.QuestDays = value,
            I18n.Config_QuestDays_Name,
            I18n.Config_QuestDays_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.MaxQuests,
            value => Config.MaxQuests = value,
            I18n.Config_MaxQuests_Name,
            I18n.Config_MaxQuests_Tooltip
        );
        // 交易任务
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_ItemDeliveryTitle_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.ItemDeliveryWeight,
            value => Config.ItemDeliveryWeight = value,
            I18n.Config_ItemDeliveryWeight_Name,
            I18n.Config_ItemDeliveryWeight_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.MustLikeItem,
            value => Config.MustLikeItem = value,
            I18n.Config_MustLikeItem_Name,
            I18n.Config_MustLikeItem_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.MustLoveItem,
            value => Config.MustLoveItem = value,
            I18n.Config_MustLoveItem_Name,
            I18n.Config_MustLoveItem_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.AllowArtisanGoods,
            value => Config.AllowArtisanGoods = value,
            I18n.Config_AllowArtisanGoods_Name,
            I18n.Config_AllowArtisanGoods_Tooltip
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.MaxPrice,
            value => Config.MaxPrice = value,
            I18n.Config_MaxPrice_Name,
            I18n.Config_MaxPrice_Tooltip
        );
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.IgnoreVanillaItemRestriction,
            value => Config.IgnoreVanillaItemRestriction = value,
            I18n.Config_IgnoreVanillaItemRestriction_Name,
            I18n.Config_IgnoreVanillaItemRestriction_Tooltip
        );
        // 采集任务
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_ResourceCollectionTitle_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.ResourceCollectionWeight,
            value => Config.ResourceCollectionWeight = value,
            I18n.Config_ResourceCollectionWeight_Name,
            I18n.Config_ResourceCollectionWeight_Tooltip
        );
        // 钓鱼任务
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_FishingTitle_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.FishingWeight,
            value => Config.FishingWeight = value,
            I18n.Config_FishingWeight_Name,
            I18n.Config_FishingWeight_Tooltip
        );
        // 杀怪任务
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_SlayMonstersTitle_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.SlayMonstersWeight,
            value => Config.SlayMonstersWeight = value,
            I18n.Config_SlayMonstersWeight_Name,
            I18n.Config_SlayMonstersWeight_Tooltip
        );
        // 外观
        configMenu.AddPageLink(
            ModManifest,
            "Appearance",
            I18n.Config_AppearancePage_Name
        );

        #region 外观

        configMenu.AddPage(
            ModManifest,
            "Appearance",
            I18n.Config_AppearancePage_Name
        );
        // 便签外观标题
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_NoteAppearanceTitle_Name
        );
        // 便签缩放
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.NoteScale,
            value => Config.NoteScale = value,
            I18n.Config_NoteScale_Name,
            I18n.Config_NoteScale_Tooltip
        );
        // 便签重叠率
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
        // 随机颜色
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.RandomColorMin,
            value => Config.RandomColorMin = value,
            I18n.Config_RandomColorMin_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.RandomColorMax,
            value => Config.RandomColorMax = value,
            I18n.Config_RandomColorMax_Name,
            null,
            0,
            255
        );
        // 肖像外观标题
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_PortraitAppearanceTitle_Name
        );
        // 肖像缩放
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitScale,
            value => Config.PortraitScale = value,
            I18n.Config_PortraitScale_Name
        );
        // 肖像偏移
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitOffsetX,
            value => Config.PortraitOffsetX = value,
            I18n.Config_PortraitOffsetX_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitOffsetY,
            value => Config.PortraitOffsetY = value,
            I18n.Config_PortraitOffsetY_Name
        );
        // 肖像颜色
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintR,
            value => Config.PortraitTintR = value,
            I18n.Config_PortraitTintR_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintG,
            value => Config.PortraitTintG = value,
            I18n.Config_PortraitTintG_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintB,
            value => Config.PortraitTintB = value,
            I18n.Config_PortraitTintB_Name,
            null,
            0,
            255
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => Config.PortraitTintA,
            value => Config.PortraitTintA = value,
            I18n.Config_PortraitTintA_Name,
            null,
            0,
            255
        );

        #endregion
    }
}