using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal abstract class BaseOption
{
    private readonly string label;
    private readonly Rectangle sourceRect;
    public float Scale { get; set; } = 1f;

    protected BaseOption(string label, Rectangle sourceRect)
    {
        this.label = label;
        this.sourceRect = sourceRect;
    }

    public virtual bool IsEnable()
    {
        return true;
    }

    public abstract void Apply();

    public void Draw(SpriteBatch b, Texture2D texture, int x, int y)
    {
        b.Draw(texture, new Vector2(x + 100, y + 100), this.sourceRect, Color.White, 0f, new Vector2(100, 100), this.Scale, SpriteEffects.None, 0f);
        DrawHelper.DrawTab(x + 100, y + 120, Game1.smallFont, this.label, Align.Center);
    }
}