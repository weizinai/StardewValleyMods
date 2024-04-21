using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;

namespace HelpWanted.Framework;

public class QuestManager
{
    private readonly ModConfig config;
    private readonly IMonitor monitor;
    private readonly Random random;

    public QuestManager(ModConfig config, IMonitor monitor)
    {
        this.config = config;
        this.monitor = monitor;
        random = new Random();
    }
    
    public void InitQuestList(List<QuestData> questList)
    {
        // 清空任务列表
        questList.Clear();
        // 如果今天没有每日任务,则刷新每日任务
        if (Game1.questOfTheDay is null) RefreshQuestOfTheDay();
    
        // 尝试次数
        var tries = 0;
        // 已有任务NPC列表
        var npcs = new List<string>();
        for (var i = 0; i < config.MaxQuests; i++)
        {
            if (Game1.questOfTheDay is null) break;
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
                if ((config.OneQuestPerVillager && npcs.Contains(npc.Name)) ||
                    (config.AvoidMaxHearts && !Game1.IsMultiplayer &&
                     Game1.player.tryGetFriendshipLevelForNPC(npc.Name) >= Utility.GetMaximumHeartsForCharacter(npc) * 250))
                {
                    tries++;
                    if (tries > 100)
                        tries = 0;
                    else
                        i--;
    
                    RefreshQuestOfTheDay();
                    continue;
                }
    
                tries = 0;
                npcs.Add(npc.Name);
                questList.Add(new QuestData(questType, npc));
            }
    
            RefreshQuestOfTheDay();
        }
    }

    private void RefreshQuestOfTheDay()
    {
        var quest = GetQuestOfTheDay();
        if (quest is null)
        {
            monitor.Log("Refresh Quest Of The Day Failed.", LogLevel.Warn);
            return;
        }
        quest.dailyQuest.Set(true);
        // 为什么要修改随机数?
        AccessTools.FieldRefAccess<Quest, Random>(quest, "random") = random;
        quest.reloadDescription();
        quest.reloadObjective();
        Game1.netWorldState.Value.SetQuestOfTheDay(quest);
    }

    private Quest? GetQuestOfTheDay()
    {
        var randomDouble = random.NextDouble();
        var slayMonsterQuest = MineShaft.lowestLevelReached > 0 && Game1.stats.DaysPlayed > 5U;
        var questTypes = new List<(float weight, Func<Quest> createQuest)>
        {
            (config.ResourceCollectionWeight, () => new ResourceCollectionQuest()),
            (slayMonsterQuest ? config.SlayMonstersWeight : 0, () => new SlayMonsterQuest()),
            (config.FishingWeight, () => new FishingQuest()),
            (config.ItemDeliveryWeight, () => new ItemDeliveryQuest())
        };
        var currentWeight = 0f;
        var totalWeight = config.ResourceCollectionWeight + (slayMonsterQuest ? config.SlayMonstersWeight : 0) + config.FishingWeight + config.ItemDeliveryWeight;
        foreach (var (weight, createQuest) in questTypes)
        {
            currentWeight += weight;
            if (randomDouble < currentWeight / totalWeight) return createQuest();
        }
        return null;
    }
}