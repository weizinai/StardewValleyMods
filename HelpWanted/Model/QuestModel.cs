using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.Model;

public class QuestModel
{
    public Texture2D Pad { get; }
    public Rectangle PadSource { get; }
    public Color PadColor { get; }
    public Texture2D Pin { get; }
    public Rectangle PinSource { get; }
    public Color PinColor { get; }
    public Texture2D Icon { get; }
    public Rectangle IconSource { get; }
    public Color IconColor { get; }
    public Quest Quest { get; }

    public float NoteWidth => this.PadSource.Width * ModConfig.Instance.NoteScale;
    public float NoteHeight => this.PadSource.Height * ModConfig.Instance.NoteScale;
    public Vector2 IconOffset => new(
        (this.NoteWidth - this.IconSource.Width * this.IconScale) / 2,
        this.NoteHeight - this.IconSource.Height * this.IconScale
    );
    public float IconScale => ModConfig.Instance.PortraitScale;

    public QuestModel(
        Texture2D pad, Rectangle padSource, Color padColor,
        Texture2D pin, Rectangle pinSource, Color pinColor,
        Texture2D icon, Rectangle iconSource, Color iconColor,
        Quest quest
    )
    {
        this.Pad = pad;
        this.PadSource = padSource;
        this.PadColor = padColor;
        this.Pin = pin;
        this.PinSource = pinSource;
        this.PinColor = pinColor;
        this.Icon = icon;
        this.IconSource = iconSource;
        this.IconColor = iconColor;
        this.Quest = quest;
    }
}