using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Quests;

namespace HelpWanted.Framework.Interface;

public interface IQuestData
{
    public Texture2D PadTexture { get; set; }
    public Rectangle PadTextureSource { get; set; }
    public Color PadColor { get; set; }
    public Texture2D PinTexture { get; set; }
    public Rectangle PinTextureSource { get; set; }
    public Color PinColor { get; set; }
    public Texture2D Icon { get; set; }
    public Rectangle IconSource { get; set; }
    public Color IconColor { get; set; }
    public float IconScale { get; set; }
    public Point IconOffset { get; set; }
    public Quest Quest { get; set; }
    public bool Acceptable { get; set; }
}