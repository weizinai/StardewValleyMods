namespace HelpWanted.Framework.Integration;

public interface IHelpWanted
{
    public void AddQuestTomorrow(IQuestData questData);
    public void AddQuestToday(IQuestData questData);
    public IList<IQuestData> GetQuests();
}