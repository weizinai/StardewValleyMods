using Common;
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
    private readonly IModHelper helper;
    private readonly AppearanceManager appearanceManager;

    public static readonly List<QuestData> VanillaQuestList = new();
    public static readonly List<QuestData> RSVQuestList = new();

    public QuestManager(IModHelper helper, ModConfig config)
    {
        this.config = config;
        this.helper = helper;
        appearanceManager = new AppearanceManager(helper, config);
    }

    public void InitVanillaQuestList()
    {
        if (Game1.stats.DaysPlayed <= 1 && !config.QuestFirstDay)
        {
            Log.Trace("今天是游戏第一天,不生成任务.");
            return;
        }

        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !config.QuestFestival)
        {
            Log.Trace("今天或明天是节日,不生成任务.");
            return;
        }

        if (Game1.random.NextDouble() >= config.DailyQuestChance)
        {
            Log.Trace("今天不生成任务.");
            return;
        }
        Log.Trace("初始化原版任务列表.");
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
    }

    public void InitRSVQuestList()
    {
        Log.Trace("初始化RSV任务列表.");
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
        return Game1.random.NextBool() ? GetRandomHandCraftedQuest() : GetRSVFishingQuest();
    }
    
    private Quest? GetRandomHandCraftedQuest()
    {
        var quests = DataLoader.Quests(Game1.content);
        var candidates = new List<string>();
        foreach(var key in quests.Keys)
        {
            if (int.TryParse(key, out int keyNr) && keyNr is >= 72861000 and <= 72862000)
            {
                if (!Game1.player.hasQuest(key))
                {
                    candidates.Add(key);
                }
            }
        }
        
        Log.Trace($"{candidates.Count} candidates for daily Quest");

        if(candidates.Count == 0)
        {
            return null;
        }

        var rand = Game1.random.Next(candidates.Count);

        Log.Trace($"chose {candidates[rand]}");
        return GetQuestFromId(candidates[rand]);
    }

    private Quest GetRSVFishingQuest()
    {
        var quest = new FishingQuest();
        quest.loadQuestInfo();

        var possibleFish = Game1.currentSeason switch
        {
            "spring" => new[]
            {
                "Rafseazz.RSVCP_Cutthroat_Trout", "Rafseazz.RSVCP_Ridgeside_Bass", "Rafseazz.RSVCP_Ridge_Bluegill", "Rafseazz.RSVCP_Caped_Tree_Frog",
                "Rafseazz.RSVCP_Pebble_Back_Crab", "Rafseazz.RSVCP_Harvester_Trout", "Rafseazz.RSVCP_Mountain_Redbelly_Dace", "Rafseazz.RSVCP_Mountain_Whitefish"
            },
            "summer" => new[]
            {
                "Rafseazz.RSVCP_Cutthroat_Trout", "Rafseazz.RSVCP_Ridgeside_Bass", "Rafseazz.RSVCP_Caped_Tree_Frog", "Rafseazz.RSVCP_Pebble_Back_Crab",
                "Rafseazz.RSVCP_Skulpin_Fish", "Rafseazz.RSVCP_Mountain_Redbelly_Dace", "Rafseazz.RSVCP_Mountain_Whitefish"
            },
            "fall" => new[]
            {
                "Rafseazz.RSVCP_Cutthroat_Trout", "Rafseazz.RSVCP_Ridgeside_Bass", "Rafseazz.RSVCP_Ridge_Bluegill", "Rafseazz.RSVCP_Caped_Tree_Frog",
                "Rafseazz.RSVCP_Pebble_Back_Crab", "Rafseazz.RSVCP_Skulpin_Fish", "Rafseazz.RSVCP_Harvester_Trout", "Rafseazz.RSVCP_Mountain_Redbelly_Dace",
                "Rafseazz.RSVCP_Mountain_Whitefish"
            },
            "winter" => new[]
            {
                "Rafseazz.RSVCP_Ridgeside_Bass", "Rafseazz.RSVCP_Ridge_Bluegill", "Rafseazz.RSVCP_Skulpin_Fish", "Rafseazz.RSVCP_Harvester_Trout",
                "Rafseazz.RSVCP_Mountain_Redbelly_Dace", "Rafseazz.RSVCP_Mountain_Whitefish"
            },
            _ => new[] { "132" }
        };

        var chosenFish = "(O)" + Game1.random.ChooseFrom(possibleFish);
        quest.ItemId.Value = chosenFish;
        if (!ItemRegistry.Exists(chosenFish))
        {
            quest.ItemId.Value = "(O)132";
        }

        var fishItem = ItemRegistry.Create(chosenFish);
        quest.numberToFish.Value = (int)Math.Ceiling(200.0 / Math.Max(1, fishItem.salePrice())) + Game1.player.FishingLevel / 5;
        quest.reward.Value = (int)(quest.numberToFish.Value + 1.5) * fishItem.salePrice();
        quest.target.Value = "Carmen";
        quest.parts.Clear();
        quest.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:Carmen.FishingQuest.Description", fishItem.DisplayName, quest.numberToFish.Value));
        quest.dialogueparts.Clear();
        quest.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:Carmen.FishingQuest.HandInDialogue", fishItem.DisplayName));
        quest.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", 0, quest.numberToFish.Value, fishItem.DisplayName);
        quest.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13274", quest.reward.Value));
        quest.parts.Add("Strings\\StringsFromCSFiles:FishingQuest.cs.13275");
        quest.daysLeft.Value = 7;
        quest.id.Value = "80000000";
        return quest;
    }
    
    private Quest? GetQuestFromId(string id)
    {
        Log.Trace($"Trying to load quest {id}");
        Quest? quest;
        try
        {
            quest = Quest.getQuestFromId(id);

            if (quest is SlayMonsterQuest monsterQuest)
            {
                Dictionary<string, string> questData = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Quests");

                if (questData != null && questData.ContainsKey(id))
                {
                    var rawData = questData[id].Split('/');
                    var conditionsSplit = rawData[4].Split(' ');

                    monsterQuest.loadQuestInfo();
                    monsterQuest.monster.Value.Name = conditionsSplit[0].Replace('_', ' ');
                    monsterQuest.monsterName.Value = monsterQuest.monster.Value.Name;
                    monsterQuest.numberToKill.Value = Convert.ToInt32(conditionsSplit[1]);
                    monsterQuest.target.Value = conditionsSplit.Length > 2 ? conditionsSplit[2] : "null";
                    monsterQuest.questType.Value = 4;
                    if (rawData.Length > 9)
                    {
                        monsterQuest.targetMessage = rawData[9];
                    }

                    monsterQuest.moneyReward.Value = Convert.ToInt32(rawData[6]);
                    monsterQuest.reward.Value = monsterQuest.moneyReward.Value;
                    monsterQuest.rewardDescription.Value = rawData[6].Equals("-1") ? null : rawData[7];
                    monsterQuest.parts.Clear();
                    monsterQuest.dialogueparts.Clear();
                    helper.Reflection.GetField<bool>(monsterQuest, "_loadedDescription").SetValue(true);
                    return monsterQuest;
                }
            }
        }
        catch(Exception e)
        {
            Log.Error($"Failed parsing quest with id {id}");
            Log.Error(e.Message);
            Log.Error(e.StackTrace);
            quest = null;
        }
			

        return quest;
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