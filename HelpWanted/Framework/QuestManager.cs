using HarmonyLib;
using HelpWanted.Patches;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;

namespace HelpWanted.Framework;

internal class QuestManager
{
    private readonly ModConfig config;
    private readonly IMonitor monitor;
    private readonly AppearanceManager appearanceManager;

    public static readonly List<QuestData> QuestList = new();

    public QuestManager(ModConfig config, IMonitor monitor, AppearanceManager appearanceManager)
    {
        this.config = config;
        this.monitor = monitor;
        this.appearanceManager = appearanceManager;
    }

    public void InitQuestList()
    {
        monitor.Log("初始化任务列表.");
        ItemDeliveryQuestPatcher.Init();
        QuestList.Clear();
        var quest = RefreshQuestOfTheDay();
        // 尝试次数
        var tries = 0;
        // 已有任务NPC列表
        var npcNames = new HashSet<string>();
        for (var i = 0; i < config.MaxQuests; i++)
        {
            if (quest is null) break;
            var npc = GetNpcFromQuest(quest);
            if (npc is not null)
            {
                monitor.Log($"第{i + 1}个任务: {quest.questTitle} - {npc.Name}");

                if (!CheckNPCAvailable(npcNames, npc))
                {
                    tries++;
                    if (tries > 100)
                        tries = 0;
                    else
                        i--;

                    quest = RefreshQuestOfTheDay();
                    continue;
                }

                tries = 0;
                npcNames.Add(npc.Name);
                AddQuest(npc, quest);
            }

            quest = RefreshQuestOfTheDay();
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

    private bool CheckNPCAvailable(HashSet<string> npcNames, NPC npc)
    {
        var oneQuestPerVillager = config.OneQuestPerVillager && npcNames.Contains(npc.Name);
        var excludeMaxHeartsNPC = config.ExcludeMaxHeartsNPC && Game1.player.tryGetFriendshipLevelForNPC(npc.Name) >= Utility.GetMaximumHeartsForCharacter(npc) * 250;
        var excludeNPCList = config.ExcludeNPCList.Contains(npc.Name);

        var available = !oneQuestPerVillager && !excludeMaxHeartsNPC && !excludeNPCList;

        if (!available)
        {
            if (oneQuestPerVillager) monitor.Log($"{npc.Name}已有任务");
            if (excludeMaxHeartsNPC) monitor.Log($"{npc.Name}好感度已满");
            if (excludeNPCList) monitor.Log($"{npc.Name}在排除列表中");
        }

        return available;
    }

    private void AddQuest(NPC npc, Quest quest)
    {
        var questType = GetQuestType(Game1.questOfTheDay);
        var padTexture = appearanceManager.GetPadTexture(npc.Name, questType.ToString());
        var padTextureSource = new Rectangle(0, 0, 64, 64);
        var padColor = appearanceManager.GetRandomColor();
        var pinTexture = appearanceManager.GetPinTexture(npc.Name, questType.ToString());
        var pinTextureSource = new Rectangle(0, 0, 64, 64);
        var pinColor = appearanceManager.GetRandomColor();
        var icon = npc.Portrait;
        var iconColor = new Color(config.PortraitTintR, config.PortraitTintB, config.PortraitTintB, config.PortraitTintA);
        var iconSource = new Rectangle(0, 0, 64, 64);
        var iconScale = config.PortraitScale;
        var iconOffset = new Point(config.PortraitOffsetX, config.PortraitOffsetY);
        var questData = new QuestData(padTexture, padTextureSource, padColor, pinTexture, pinTextureSource, pinColor, icon, iconSource, iconColor, iconScale, iconOffset,
            quest);
        QuestList.Add(questData);
    }

    private Quest? RefreshQuestOfTheDay()
    {
        var quest = GetQuestOfTheDay();
        if (quest is null)
        {
            monitor.Log("Refresh Quest Of The Day Failed.", LogLevel.Warn);
            return null;
        }

        quest.daysLeft.Value = config.QuestDays;
        quest.dailyQuest.Value = true;
        quest.accepted.Value = true;
        quest.canBeCancelled.Value = true;
        AccessTools.FieldRefAccess<Quest, Random>(quest, "random") = Game1.random;
        quest.reloadDescription();
        quest.reloadObjective();
        monitor.Log("随机生成每日任务");
        return quest;
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

public enum QuestType
{
    ItemDelivery,
    ResourceCollection,
    SlayMonster,
    Fishing,
    Unknown
}