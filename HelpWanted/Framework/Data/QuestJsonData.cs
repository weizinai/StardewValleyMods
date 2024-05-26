using StardewValley;

namespace HelpWanted.Framework.Data;

internal class QuestJsonData
{
    public QuestType QuestType { get; set; }
    public string Requirement { get; set; } = null!;
    public int Number { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Target { get; set; } = null!;
    public string TargetMessage { get; set; } = null!;
    public string CurrentObjective { get; set; } = null!;
    public string Condition { get; set; } = null!;
}