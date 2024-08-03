using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal abstract class BaseOption
{
    public string Label { get; }
    public Rectangle SourceRect { get; }
    public float Scale { get; set; } = 1f;

    protected BaseOption(string label, Rectangle sourceRect)
    {
        this.Label = label;
        this.SourceRect = sourceRect;
    }

    public virtual bool IsEnable()
    {
        return true;
    }

    public abstract void Apply();

    public void Draw(SpriteBatch b, Texture2D texture, int x, int y)
    {
        b.Draw(texture, new Vector2(x + 100, y + 100), this.SourceRect, Color.White, 0f, new Vector2(100, 100), this.Scale, SpriteEffects.None, 0f);
        DrawHelper.DrawTab(x + 100, y + 120, Game1.smallFont, this.Label, Align.Center);
    }
}