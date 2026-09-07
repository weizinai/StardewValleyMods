using System.Collections.Generic;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

public class VanillaModConfig
{
    // 一般设置
    public bool QuestFirstDay { get; set; }
    public bool QuestFestival { get; set; }
    public float DailyQuestChance { get; set; } = 0.9f;
    public bool OneQuestPerVillager { get; set; }
    public bool ExcludeMaxHeartsNPC { get; set; }
    public List<string> ExcludeNPCList { get; set; } = new();
    public int MaxQuests { get; set; } = 10;

    // 交易任务
    public BaseQuestConfig ItemDeliveryQuestConfig { get; set; } = new(0.4f, 1f, 2);
    public int ItemDeliveryFriendshipGain { get; set; } = 150;
    public bool RewriteQuestItem { get; set; } = true;
    public int QuestItemRequirement { get; set; } = 1;
    public bool AllowArtisanGoods { get; set; } = true;
    public int MaxPrice { get; set; } = -1;

    // 采集任务
    public BaseQuestConfig ResourceCollectionQuestConfig { get; set; } = new(0.08f, 1f, 2);
    public bool MoreResourceCollectionQuest { get; set; } = true;

    // 钓鱼任务
    public BaseQuestConfig FishingQuestConfig { get; set; } = new(0.07f, 1f, 2);

    // 杀怪任务
    public BaseQuestConfig SlayMonsterQuestConfig { get; set; } = new(0.1f, 1f, 2);
    public bool MoreSlayMonsterQuest { get; set; } = true;
}
