﻿using Common;
using HarmonyLib;
using HelpWanted.Patches;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Quests;

namespace HelpWanted.Framework;

internal class QuestManager
{
    private readonly ModConfig config;
    private readonly AppearanceManager appearanceManager;

    public static readonly List<QuestData> VanillaQuestList = new();
    public static readonly List<QuestData> RSVQuestList = new();

    public QuestManager(IModHelper helper, ModConfig config)
    {
        this.config = config;
        appearanceManager = new AppearanceManager(helper, config);
    }

    public void InitVanillaQuestList()
    {
        if (Game1.stats.DaysPlayed <= 1 && !config.QuestFirstDay)
        {
            Log.Info("Today is the first day of the game, no daily quests are generated.");
            return;
        }

        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !config.QuestFestival)
        {
            Log.Info("Today or tomorrow is the festival day, no daily quests are generated.");
            return;
        }

        if (Game1.random.NextDouble() >= config.DailyQuestChance)
        {
            Log.Trace("No daily quests are generated.");
            return;
        }

        Log.Info("Begin generating today's daily quests.");
        ItemDeliveryQuestPatcher.Init();
        VanillaQuestList.Clear();
        var quest = GetVanillaQuest();
        var tries = 0;
        var npcNames = new HashSet<string>();
        for (var i = 0; i < config.MaxQuests; i++)
        {
            if (quest is null) break;
            var npc = GetNpcFromQuest(quest);
            if (npc is not null)
            {
                Log.Trace($"第{i + 1}个任务: {quest.questTitle} - {npc.Name}");

                if (!CheckNPCAvailable(npcNames, npc))
                {
                    tries++;
                    if (tries > 100)
                        tries = 0;
                    else
                        i--;

                    quest = GetVanillaQuest();
                    continue;
                }

                tries = 0;
                npcNames.Add(npc.Name);
                VanillaQuestList.Add(GetQuestData(npc, quest));
            }

            quest = GetVanillaQuest();
        }

        Log.Info("End generating today's daily quests.");
    }

    public void InitRSVQuestList()
    {
        Log.Info("Begin generating today's daily quests of RSV.");
        RSVQuestList.Clear();
        var quest = GetRSVQuest();
        for (var i = 0; i < 5; i++)
        {
            if (quest is null) break;
            var npc = GetNpcFromQuest(quest);
            if (npc is not null)
            {
                Log.Trace($"第{i + 1}个任务: {quest.questTitle} - {npc.Name}");
                RSVQuestList.Add(GetQuestData(npc, quest));
            }

            quest = GetRSVQuest();
        }

        Log.Info("End generating today's daily quests of RSV.");
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
            if (oneQuestPerVillager) Log.Trace($"{npc.Name}已有任务");
            if (excludeMaxHeartsNPC) Log.Trace($"{npc.Name}好感度已满");
            if (excludeNPCList) Log.Trace($"{npc.Name}在排除列表中");
        }

        return available;
    }

    private QuestData GetQuestData(NPC npc, Quest quest)
    {
        var questType = GetQuestType(quest);
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
        return new QuestData(padTexture, padTextureSource, padColor, pinTexture, pinTextureSource, pinColor,
            icon, iconSource, iconColor, iconScale, iconOffset, quest);
    }

    private Quest? GetVanillaQuest()
    {
        Quest? quest = null;

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
            if (randomDouble < currentWeight / totalWeight) quest = createQuest();
        }

        if (quest is null)
        {
            Log.Warn("Get Vanilla Quest Failed.");
            return null;
        }

        quest.daysLeft.Value = config.QuestDays;
        quest.dailyQuest.Value = true;
        quest.accepted.Value = true;
        quest.canBeCancelled.Value = true;
        AccessTools.FieldRefAccess<Quest, Random>(quest, "random") = Game1.random;
        quest.reloadDescription();
        quest.reloadObjective();
        Log.Trace("Get Vanilla Quest Succeed.");
        return quest;
    }

    private Quest? GetRSVQuest()
    {
        return Game1.random.NextBool()
            ? AccessTools.Method(Type.GetType("RidgesideVillage.Questing.QuestFactory,RidgesideVillage"), "GetRandomHandCraftedQuest").Invoke(null, null) as Quest
            : AccessTools.Method(Type.GetType("RidgesideVillage.Questing.QuestFactory,RidgesideVillage"), "GetFishingQuest").Invoke(null, null) as Quest;
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