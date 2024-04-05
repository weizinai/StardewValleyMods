using HarmonyLib;
using HelpWanted.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;

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
    public static readonly List<IQuestData> QuestList = new();

    /// <summary>其他模组添加的求助任务</summary>
    private readonly List<IQuestData> modQuestList = new();

    public static bool GettingQuestDetails;

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);

        SHelper = helper;
        SMonitor = Monitor;
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.Content.AssetRequested += OnAssetRequested;
        helper.Events.GameLoop.DayStarted += OnDayStarted;

        HarmonyPatch();
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
        var dictionary = SHelper.GameContent.Load<Dictionary<string, QuestJsonData>>(DictionaryPath);
        foreach (var kvp in dictionary)
        {
            var data = kvp.Value;
            if (Game1.random.Next(100) >= data.PercentChance)
                continue;
            modQuestList.Add(new QuestData(data));
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
            if (modQuestList.Any())
            {
                QuestList.Add(modQuestList[0]);
                modQuestList.RemoveAt(0);
                continue;
            }

            try
            {
                if (Game1.questOfTheDay != null)
                {
                    AccessTools.FieldRefAccess<Quest, Random>(Game1.questOfTheDay, "random") = Random;
                    GettingQuestDetails = true;
                    Game1.questOfTheDay.reloadDescription();
                    Game1.questOfTheDay.reloadObjective();
                    GettingQuestDetails = false;
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
                        AddQuest(Game1.questOfTheDay, questType, icon);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error loading quest:\n\n {ex}", LogLevel.Warn);
            }

            RefreshQuestOfTheDay();
        }

        modQuestList.Clear();
        SHelper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
    }
}