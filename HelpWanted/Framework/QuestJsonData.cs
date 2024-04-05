using Microsoft.Xna.Framework;
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

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
    public string? PadTexturePath = null;
    public Rectangle PinTextureSource = new(0, 0, 64, 64);
    public Color? PadColor = null;
    public Rectangle PadTextureSource = new(0, 0, 64, 64);
    public string? PinTexturePath = null;
    public Color? PinColor = null;
    public string? IconPath = null;
    public Rectangle IconSource = new(0, 0, 64, 64);
    public Color? IconColor = null;
    public float IconScale = 1;
    public Point? IconOffset = null;
    public QuestInfo QuestInfo;
    public float PercentChance = 100;
}

public class QuestInfo
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