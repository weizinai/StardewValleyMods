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

    public QuestManager(ModConfig config, IMonitor monitor)
    {
        this.config = config;
        this.monitor = monitor;
    }
    
    public void InitQuestList(List<QuestData> questList)
    {
        questList.Clear();
        if (Game1.questOfTheDay is null) RefreshQuestOfTheDay();
        // 尝试次数
        var tries = 0;
        // 已有任务NPC列表
        var npcs = new HashSet<string>();
        for (var i = 0; i < config.MaxQuests; i++)
        {
            if (Game1.questOfTheDay is null) break;
            var npc = GetNpcFromQuest(Game1.questOfTheDay);
            if (npc is not null)
            {
                if ((config.OneQuestPerVillager && npcs.Contains(npc.Name)) ||
                    (config.AvoidMaxHearts && Game1.MasterPlayer.tryGetFriendshipLevelForNPC(npc.Name) >= Utility.GetMaximumHeartsForCharacter(npc) * 250))
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
                questList.Add(new QuestData(GetQuestType(Game1.questOfTheDay), npc));
            }
            RefreshQuestOfTheDay();
        }
    }
    
    private NPC? GetNpcFromQuest(Quest quest)
    {
        return quest switch
        {
            ItemDeliveryQuest itemDeliveryQuest => Game1.getCharacterFromName(itemDeliveryQuest.target.Value),
            ResourceCollectionQuest resourceCollectionQuest => Game1.getCharacterFromName(resourceCollectionQuest.target.Value),
            SlayMonsterQuest slayMonsterQuest => Game1.getCharacterFromName(slayMonsterQuest.target.Value),
            FishingQuest fishingQuest => Game1.getCharacterFromName(fishingQuest.target.Value),
            _ => null
        };
    }
    
    private QuestType GetQuestType(Quest quest)
    {
        return quest switch
        {
            ItemDeliveryQuest => QuestType.ItemDelivery,
            ResourceCollectionQuest => QuestType.ResourceCollection,
            SlayMonsterQuest => QuestType.SlayMonster,
            FishingQuest => QuestType.Fishing,
            _ => QuestType.Unknown
        };
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
        AccessTools.FieldRefAccess<Quest, Random>(quest, "random") = Game1.random;
        quest.reloadDescription();
        quest.reloadObjective();
        Game1.netWorldState.Value.SetQuestOfTheDay(quest);
    }

    private Quest? GetQuestOfTheDay()
    {
        var randomDouble = Game1.random.NextDouble();
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