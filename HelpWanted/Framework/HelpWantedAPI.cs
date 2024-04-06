using HelpWanted.Framework.Interface;
using StardewValley.Quests;

namespace HelpWanted.Framework;

public class HelpWantedAPI : IHelpWanted
{
    public void AddQuestTomorrow(IQuestData questData)
    {
        ModEntry.SMonitor.Log($"Adding mod quest data {questData.Quest.GetType()}");
        ModEntry.ModQuestList.Add(questData);
    }

    public void AddQuestToday(IQuestData questData)
    {
        ModEntry.SMonitor.Log($"Adding quest data {questData.Quest.GetType()}");
        var questType = questData.Quest switch
        {
            ResourceCollectionQuest => QuestType.ResourceCollection,
            SlayMonsterQuest => QuestType.SlayMonster,
            FishingQuest => QuestType.Fishing,
            _ => QuestType.ItemDelivery
        };
        if (ModEntry.QuestList.Count == 0) ModEntry.QuestList = OrdersBillboard.QuestDataDictionary.Values.ToList();
        ModEntry.AddQuest(questData.Quest, questType, questData.Icon, questData.IconSource, questData.IconOffset);
    }

    public IList<IQuestData> GetQuests()
    {
        ModEntry.SMonitor.Log("Getting quest list");
        return ModEntry.ModQuestList;
    }
}