namespace weizinai.StardewValleyMod.HelpWanted.Framework;

public class RSVModConfig
{
    // General Setting
    public bool EnableRSVQuestBoard { get; set; } = true;
    public bool QuestFirstDay { get; set; }
    public bool QuestFestival { get; set; }
    public float DailyQuestChance { get; set; } = 0.8f;
    public int MaxQuests { get; set; } = 10;
    public bool AllowSameQuest { get; set; }

    // Item Delivery Quest
    public BaseQuestConfig ItemDeliveryQuestConfig { get; set; } = new(0.27f, 1f, 0);

    // Fishing Quest
    public BaseQuestConfig FishingQuestConfig { get; set; } = new(0.27f, 1f, 7);

    // Slay Monster Quest
    public BaseQuestConfig SlayMonsterQuestConfig { get; set; } = new(0.27f, 1f, 0);

    // Lost Item Quest
    public BaseQuestConfig LostItemQuestConfig { get; set; } = new(0.2f, 1f, 0);
}
