using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.HelpWanted.Framework.Data;
using weizinai.StardewValleyMod.HelpWanted.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

internal class QuestManager
{
    private readonly IModHelper helper;
    private readonly AppearanceManager appearanceManager;

    public static readonly List<QuestData> VanillaQuestList = new();
    public static readonly List<QuestData> RSVQuestList = new();

    private Dictionary<string, QuestJsonData> rawCustomQuestData = new();
    private readonly Dictionary<QuestType, List<QuestData>> customQuestData = new();

    private const string CustomQuestDataPath = "weizinai.HelpWanted/Quest";

    public QuestManager(IModHelper helper)
    {
        // 初始化
        this.helper = helper;
        this.appearanceManager = new AppearanceManager(helper);
        // 注册事件
        helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(CustomQuestDataPath))
            e.LoadFrom(() => new Dictionary<string, QuestJsonData>(), AssetLoadPriority.Exclusive);
    }

    public void InitVanillaQuestList()
    {
        if (!this.CheckDayAvailable()) return;

        Log.Info("Begin generating today's daily quests.");
        ItemDeliveryQuestPatcher.Init();
        VanillaQuestList.Clear();
        var quest = this.GetVanillaQuest();
        var tries = 0;
        var npcNames = new HashSet<string>();
        for (var i = 0; i < ModConfig.Instance.MaxQuests; i++)
        {
            if (quest is null) break;
            var npc = this.GetNpcFromQuest(quest);
            if (npc is not null)
            {
                Log.Trace($"第{i + 1}个任务: {quest.questTitle} - {npc.Name}");

                if (!this.CheckNPCAvailable(npcNames, npc))
                {
                    tries++;
                    if (tries > 100)
                        tries = 0;
                    else
                        i--;

                    quest = this.GetVanillaQuest();
                    continue;
                }

                tries = 0;
                npcNames.Add(npc.Name);
                VanillaQuestList.Add(this.GetQuestData(npc, quest));
                Log.Trace("成功添加一个任务到原版任务列表");
            }

            quest = this.GetVanillaQuest();
        }

        Log.Info("End generating today's daily quests.");
    }

    public void InitRSVQuestList()
    {
        Log.Info("Begin generating today's daily quests of RSV.");
        RSVQuestList.Clear();
        var quest = this.GetRSVQuest();
        for (var i = 0; i < ModConfig.Instance.MaxRSVQuests; i++)
        {
            if (quest is null)
            {
                quest = this.GetRSVQuest();
                continue;
            }

            var npc = this.GetNpcFromQuest(quest);
            if (npc is not null)
            {
                Log.Trace($"第{i + 1}个任务: {quest.questTitle} - {npc.Name}");
                RSVQuestList.Add(this.GetQuestData(npc, quest));
                Log.Trace("成功添加一个任务到RSV任务列表");
            }

            quest = this.GetRSVQuest();
        }

        Log.Info("End generating today's daily quests of RSV.");
    }

    private void InitCustomQuestData()
    {
        this.rawCustomQuestData = this.helper.GameContent.Load<Dictionary<string, QuestJsonData>>(CustomQuestDataPath);
        foreach (var (id, questData) in this.rawCustomQuestData)
        {
            if (!GameStateQuery.CheckConditions(questData.Condition)) continue;
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

    private bool CheckDayAvailable()
    {
        if (Game1.stats.DaysPlayed <= 1 && !ModConfig.Instance.QuestFirstDay)
        {
            Log.Info("Today is the first day of the game, no daily quests are generated.");
            return false;
        }

        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !ModConfig.Instance.QuestFestival)
        {
            Log.Info("Today or tomorrow is the festival day, no daily quests are generated.");
            return false;
        }

        if (Game1.random.NextDouble() >= ModConfig.Instance.DailyQuestChance)
        {
            Log.Trace("No daily quests are generated.");
            return false;
        }

        return true;
    }

    private bool CheckNPCAvailable(HashSet<string> npcNames, NPC npc)
    {
        var oneQuestPerVillager = ModConfig.Instance.OneQuestPerVillager && npcNames.Contains(npc.Name);
        var excludeMaxHeartsNPC = ModConfig.Instance.ExcludeMaxHeartsNPC && Game1.player.tryGetFriendshipLevelForNPC(npc.Name) >= Utility.GetMaximumHeartsForCharacter(npc) * 250;
        var excludeNPCList = ModConfig.Instance.ExcludeNPCList.Contains(npc.Name);

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
        var questType = this.GetQuestType(quest);
        var padTexture = this.appearanceManager.GetPadTexture(npc.Name, questType.ToString());
        var padTextureSource = new Rectangle(0, 0, 64, 64);
        var padColor = this.appearanceManager.GetRandomColor();
        var pinTexture = this.appearanceManager.GetPinTexture(npc.Name, questType.ToString());
        var pinTextureSource = new Rectangle(0, 0, 64, 64);
        var pinColor = this.appearanceManager.GetRandomColor();
        var icon = npc.Portrait;
        var iconColor = new Color(ModConfig.Instance.PortraitTintR, ModConfig.Instance.PortraitTintB, ModConfig.Instance.PortraitTintB, ModConfig.Instance.PortraitTintA);
        var iconSource = new Rectangle(0, 0, 64, 64);
        var iconScale = ModConfig.Instance.PortraitScale;
        var iconOffset = new Point(ModConfig.Instance.PortraitOffsetX, ModConfig.Instance.PortraitOffsetY);
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
            (ModConfig.Instance.ResourceCollectionWeight, () => new ResourceCollectionQuest()),
            (slayMonsterQuest ? ModConfig.Instance.SlayMonstersWeight : 0, () => new SlayMonsterQuest()),
            (ModConfig.Instance.FishingWeight, () => new FishingQuest()),
            (ModConfig.Instance.ItemDeliveryWeight, () => new ItemDeliveryQuest())
        };
        var currentWeight = 0f;
        var totalWeight = ModConfig.Instance.ResourceCollectionWeight + (slayMonsterQuest ? ModConfig.Instance.SlayMonstersWeight : 0) + ModConfig.Instance.FishingWeight +
                          ModConfig.Instance.ItemDeliveryWeight;
        foreach (var (weight, createQuest) in questTypes)
        {
            Log.Trace($"{this.GetQuestType(createQuest())}的权重为{weight}");
            currentWeight += weight;
            if (randomDouble < currentWeight / totalWeight)
            {
                quest = createQuest();
                Log.Trace($"随机到一个{quest}任务");
                break;
            }
        }

        if (quest is null)
        {
            Log.Warn("Get Vanilla Quest Failed.");
            return null;
        }

        quest.daysLeft.Value = ModConfig.Instance.QuestDays;
        quest.daysLeft.Value = ModConfig.Instance.QuestDays;
        quest.dailyQuest.Value = true;
        quest.accepted.Value = true;
        quest.canBeCancelled.Value = true;
        quest.reloadDescription();
        quest.reloadObjective();
        Log.Trace($"成功获取一个原版{this.GetQuestType(quest)}任务");
        return quest;
    }

    private Quest? GetRSVQuest()
    {
        return Game1.random.NextBool()
            ? AccessTools.Method(Type.GetType("RidgesideVillage.Questing.QuestFactory,RidgesideVillage"), "GetRandomHandCraftedQuest").Invoke(null, null) as Quest
            : AccessTools.Method(Type.GetType("RidgesideVillage.Questing.QuestFactory,RidgesideVillage"), "GetFishingQuest").Invoke(null, null) as Quest;
    }
}