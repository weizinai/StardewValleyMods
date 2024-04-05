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

public abstract class QuestJsonData
{
    public readonly string? PadTexturePath = null;
    public Rectangle PinTextureSource = new(0, 0, 64, 64);
    public Color? PadColor = null;
    public Rectangle PadTextureSource = new(0, 0, 64, 64);
    public readonly string? PinTexturePath = null;
    public Color? PinColor = null;
    public readonly string? IconPath = null;
    public Rectangle IconSource = new(0, 0, 64, 64);
    public Color? IconColor = null;
    public readonly float IconScale = 1;
    public Point? IconOffset = null;
    public readonly QuestInfo Quest;
    public readonly float PercentChance = 100;
}

public abstract class QuestInfo
{
    public QuestType QuestType;

    /// <summary>The qualified item ID that must be collected.</summary>
    public string ItemId;

    /// <summary>The number of items.</summary>
    public int Number;

    public string QuestTitle;
    public string QuestDescription;

    /// <summary>The internal name for the NPC who gave the quest.</summary>
    public string Target;

    /// <summary>The translated NPC dialogue shown when the quest is completed.</summary>
    public string TargetMessage;

    public string CurrentObjective;

}