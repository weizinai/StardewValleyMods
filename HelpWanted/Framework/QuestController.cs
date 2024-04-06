using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;

namespace HelpWanted.Framework;

public class QuestController
{
    private readonly ModConfig config;

    public QuestController(ModConfig config)
    {
        this.config = config;
    }

    /// <summary>刷新每日任务</summary>
    public void RefreshQuestOfTheDay()
    {
        if (Game1.stats.DaysPlayed <= 1) return;

        // 玩家在矿井中达到的最大层数大于0并且游戏天数大于5天.则可以接到杀怪任务
        var mine = MineShaft.lowestLevelReached > 0 && Game1.stats.DaysPlayed > 5U;

        // 总权重
        var totalWeight = config.ResourceCollectionWeight + (mine ? config.SlayMonstersWeight : 0) +
                          config.FishingWeight + config.ItemDeliveryWeight;

        // 生成一个0-1之间的随机双浮点数
        var randomDouble = Game1.random.NextDouble();
        double currentWeight = 0;
        var questTypes = new List<(double weight, Func<Quest> createQuest)>
        {
            (config.ResourceCollectionWeight, () => new ResourceCollectionQuest()),
            (mine ? config.SlayMonstersWeight : 0, () => new SlayMonsterQuest()),
            (config.FishingWeight, () => new FishingQuest()),
            (config.ItemDeliveryWeight, () => new ItemDeliveryQuest())
        };
        foreach (var (weight, createQuest) in questTypes)
        {
            currentWeight += weight;
            if (randomDouble < currentWeight / totalWeight)
            {
                Game1.netWorldState.Value.SetQuestOfTheDay(createQuest());
                return;
            }
        }
    }
}