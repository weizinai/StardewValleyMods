using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Quests;

namespace HelpWanted.Framework;

public class QuestData : IQuestData
{
    public QuestData(QuestJsonData data)
    {
        var config = ModEntry.Config;
        var helper = ModEntry.SHelper;
        PinTexture = string.IsNullOrEmpty(data.PinTexturePath)
            ? ModEntry.GetPinTexture(data.Quest.Target, data.Quest.QuestType.ToString())
            : helper.GameContent.Load<Texture2D>(data.PinTexturePath);
        PadTextureSource = data.PadTextureSource;
        PinColor = data.PinColor ?? ModEntry.GetRandomColor();
        PadTexture = string.IsNullOrEmpty(data.PadTexturePath)
            ? ModEntry.GetPadTexture(data.Quest.Target, data.Quest.QuestType.ToString())
            : helper.GameContent.Load<Texture2D>(data.PadTexturePath);
        PinTextureSource = data.PinTextureSource;
        PadColor = data.PadColor ?? ModEntry.GetRandomColor();
        Icon = string.IsNullOrEmpty(data.IconPath)
            ? Game1.getCharacterFromName(data.Quest.Target).Portrait
            : ModEntry.SHelper.GameContent.Load<Texture2D>(data.IconPath);
        IconSource = data.IconSource;
        IconColor = data.IconColor ?? new Color(config.PortraitTintR, config.PortraitTintG, config.PortraitTintB, config.PortraitTintA);
        IconScale = data.IconScale;
        IconOffset = data.IconOffset ?? new Point(config.PortraitOffsetX, config.PortraitOffsetY);
        Quest = ModEntry.CreateQuest(data.Quest);
    }

    public QuestData(Texture2D padTexture, Texture2D pinTexture, Texture2D icon)
    {
        var config = ModEntry.Config;
        PadTexture = padTexture;    
        PadTextureSource = new Rectangle(0, 0, 64, 64);
        PadColor = ModEntry.GetRandomColor();
        PinTexture = pinTexture;
        PinTextureSource = new Rectangle(0, 0, 64, 64);
        PinColor = ModEntry.GetRandomColor();
        Icon = icon;
        IconSource = new Rectangle(0,0,64,64);
        IconColor = new Color(config.PortraitTintR, config.PortraitTintG, config.PortraitTintB, config.PortraitTintA);
        IconScale = config.PortraitScale;
        IconOffset = new Point(config.PortraitOffsetX, config.PortraitOffsetY);
        Quest = Game1.questOfTheDay;
    }

    public Texture2D PadTexture { get; set; }
    public Rectangle PadTextureSource { get; set; }
    public Color PadColor { get; set; }
    public Texture2D PinTexture { get; set; }
    public Rectangle PinTextureSource { get; set; }
    public Color PinColor { get; set; }
    public Texture2D? Icon { get; set; }
    public Rectangle IconSource { get; set; }
    public Color IconColor { get; set; }
    public float IconScale { get; set; }
    public Point IconOffset { get; set; }
    public Quest Quest { get; set; }
    public bool Acceptable { get; set; } = true;
}