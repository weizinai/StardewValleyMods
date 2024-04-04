using Microsoft.Xna.Framework;
using Netcode;

namespace HelpWanted.Framework;

public enum QuestType
{
    ItemDelivery,
    ResourceCollection,
    SlayMonster,
    Fishing
}

public class QuestJsonData
{
    public Rectangle PinTextureSource = new Rectangle(0, 0, 64, 64);
    public Rectangle PadTextureSource = new Rectangle(0, 0, 64, 64);
    public string pinTexturePath;
    public string padTexturePath;
    public Color? pinColor;
    public Color? padColor;
    public string iconPath;
    public Rectangle iconSource = new Rectangle(0, 0, 64, 64);
    public Color? iconColor;
    public float iconScale = 1;
    public Point? iconOffset;
    public QuestInfo Quest;
    public float percentChance = 100;
}

public class QuestInfo
{
    public QuestType QuestType { get; set; }

    /// <summary>The qualified item ID that must be collected.</summary>
    public string ItemId { get; set; }

    /// <summary>The number of items.</summary>
    public int Number { get; set; }

    public string QuestTitle { get; set; }
    public string QuestDescription { get; set; }

    /// <summary>The internal name for the NPC who gave the quest.</summary>
    public string Target { get; set; }

    /// <summary>The translated NPC dialogue shown when the quest is completed.</summary>
    public string TargetMessage { get; set; }

    public string CurrentObjective { get; set; }

    public QuestInfo(QuestType questType, NetString itemId, NetInt number, string questTitle, string questDescription, 
        NetString target, NetString targetMessage, string currentObjective)
    {
        QuestType = questType;
        ItemId = itemId.Value;
        Number = number.Value;
        QuestTitle = questTitle;
        QuestDescription = questDescription;
        Target = target.Value;
        TargetMessage = targetMessage.Value;
        CurrentObjective = currentObjective;
    }
}