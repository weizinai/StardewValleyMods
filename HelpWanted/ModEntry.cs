using Common;
using HarmonyLib;
using HelpWanted.Framework;
using HelpWanted.Framework.Integration;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace HelpWanted;

internal partial class ModEntry : Mod
{
    public static IModHelper SHelper;
    public static IMonitor SMonitor;

    public static ModConfig Config = new();
    private const string DictionaryPath = "aedenthorn.HelpWanted/Dictionary";
    private const string PadTexturePath = "aedenthorn.HelpWanted/Pin";
    private const string PinTexturePath = "aedenthorn.HelpWanted/Pad";

    private static readonly Random Random = new();
    public static List<IQuestData> QuestList = new();

    /// <summary>其他模组添加的求助任务</summary>
    public static readonly List<IQuestData> ModQuestList = new();

    private static bool gettingQuestDetails;

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);

        SHelper = helper;
        SMonitor = Monitor;
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.Content.AssetRequested += OnAssetRequested;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;

        HarmonyPatch();
    }

    public override object GetApi()
    {
        return new HelpWantedAPI();
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        // 模组未启用
        if (!Config.ModEnabled)
            return;
        
        if (e.NameWithoutLocale.IsEquivalentTo(DictionaryPath))
             e.LoadFrom(() => new Dictionary<string, QuestJsonData>(), AssetLoadPriority.Exclusive);
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        // 模组未启用或者今天是节日
        if (!Config.ModEnabled || Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
            return;
        // 将其他模组添加的求助任务加载到 modQuestList 中
        var dictionary = Helper.GameContent.Load<Dictionary<string, QuestJsonData>>(DictionaryPath);
        foreach (var kvp in dictionary)
        {
            var data = kvp.Value;
            if (Game1.random.Next(100) >= data.PercentChance)
                continue;
            ModQuestList.Add(new QuestData(data));
        }

        SHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        QuestList.Clear();
        var npcs = new List<string>();
        // 如果今天没有求助任务，则刷新求助任务
        if (Game1.questOfTheDay is null) RefreshQuestOfTheDay();

        var tries = 0;
        for (var i = 0; i < Config.MaxQuests; i++)
        {
            if (ModQuestList.Any())
            {
                QuestList.Add(ModQuestList[0]);
                ModQuestList.RemoveAt(0);
                continue;
            }

            try
            {
                if (Game1.questOfTheDay != null)
                {
                    AccessTools.FieldRefAccess<Quest, Random>(Game1.questOfTheDay, "random") = Random;
                    gettingQuestDetails = true;
                    Game1.questOfTheDay.reloadDescription();
                    Game1.questOfTheDay.reloadObjective();
                    gettingQuestDetails = false;
                    NPC? npc = null;
                    var questType = QuestType.ItemDelivery;
                    switch (Game1.questOfTheDay)
                    {
                        case ItemDeliveryQuest itemDeliveryQuest:
                            npc = Game1.getCharacterFromName(itemDeliveryQuest.target.Value);
                            break;
                        case ResourceCollectionQuest resourceCollectionQuest:
                            npc = Game1.getCharacterFromName(resourceCollectionQuest.target.Value);
                            questType = QuestType.ResourceCollection;
                            break;
                        case SlayMonsterQuest slayMonsterQuest:
                            npc = Game1.getCharacterFromName(slayMonsterQuest.target.Value);
                            questType = QuestType.SlayMonster;
                            break;
                        case FishingQuest fishingQuest:
                            npc = Game1.getCharacterFromName(fishingQuest.target.Value);
                            questType = QuestType.Fishing;
                            break;
                    }

                    if (npc is not null)
                    {
                        if ((Config.OneQuestPerVillager && npcs.Contains(npc.Name)) ||
                            (Config.AvoidMaxHearts && !Game1.IsMultiplayer &&
                             Game1.player.tryGetFriendshipLevelForNPC(npc.Name) >= Utility.GetMaximumHeartsForCharacter(npc) * 250))
                        {
                            tries++;
                            if (tries > 100)
                            {
                                tries = 0;
                            }
                            else
                            {
                                i--;
                            }

                            RefreshQuestOfTheDay();
                            continue;
                        }

                        tries = 0;
                        npcs.Add(npc.Name);
                        var icon = npc.Portrait;
                        var iconSource = new Rectangle(0, 0, 64, 64);
                        var iconOffset = new Point(Config.PortraitOffsetX, Config.PortraitOffsetY);
                        AddQuest(Game1.questOfTheDay, questType, icon, iconSource, iconOffset);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error loading quest:\n\n {ex}", LogLevel.Warn);
            }

            RefreshQuestOfTheDay();
        }

        ModQuestList.Clear();
        SHelper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // 获取GMCM提供的API
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // 注册配置菜单
        configMenu.Register(
            ModManifest,
            () => Config = Helper.ReadConfig<ModConfig>(),
            () => Helper.WriteConfig(Config)
        );

        // 添加ModEnable配置选项
        configMenu.AddBoolOption(
            ModManifest,
            () => Config.ModEnabled,
            value => Config.ModEnabled = value,
            I18n.Config_ModEnabled_Name,
            I18n.Config_ModEnabled_Tooltip
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