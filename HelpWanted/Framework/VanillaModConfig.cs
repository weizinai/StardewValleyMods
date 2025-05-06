using System.Collections.Generic;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

public class VanillaModConfig
{
    // 一般设置
    public bool QuestFirstDay;
    public bool QuestFestival;
    public float DailyQuestChance = 0.9f;
    public bool OneQuestPerVillager;
    public bool ExcludeMaxHeartsNPC;
    public List<string> ExcludeNPCList = new();
    public int MaxQuests = 10;

    // 交易任务
    public BaseQuestConfig ItemDeliveryQuestConfig { get; set; } = new(0.4f, 1f, 2);
    public int ItemDeliveryFriendshipGain = 150;
    public bool RewriteQuestItem = true;
    public int QuestItemRequirement = 1;
    public bool AllowArtisanGoods = true;
    public int MaxPrice = -1;

    // 采集任务
    public BaseQuestConfig ResourceCollectionQuestConfig { get; set; } = new(0.08f, 1f, 2);
    public bool MoreResourceCollectionQuest = true;

    // 钓鱼任务
    public BaseQuestConfig FishingQuestConfig { get; set; } = new(0.07f, 1f, 2);

    // 杀怪任务
    public BaseQuestConfig SlayMonsterQuestConfig { get; set; } = new(0.1f, 1f, 2);
    public bool MoreSlayMonsterQuest = true;
}