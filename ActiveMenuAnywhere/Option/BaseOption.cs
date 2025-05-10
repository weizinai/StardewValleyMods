using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public abstract class BaseOption
{
    private readonly string label;
    private readonly Texture2D texture;
    private readonly Rectangle sourceRect;
    public float Scale { get; set; } = 1f;

    public OptionId Id { get; }

    protected BaseOption(string label, Texture2D texture, Rectangle sourceRect, OptionId id)
    {
        this.label = label;
        this.texture = texture;
        this.sourceRect = sourceRect;
        this.Id = id;
    }

    public virtual bool IsEnable()
    {
        return true;
    }

    public abstract void Apply();

    public void Draw(SpriteBatch b, int x, int y)
    {
        b.Draw(
            this.texture,
            new Vector2(x + 100, y + 100),
            this.sourceRect, Color.White,
            0f,
            new Vector2(100, 100),
            this.Scale,
            SpriteEffects.None,
            0f
        );

        DrawHelper.DrawTab(x + 100, y + 120, Game1.smallFont, this.label, Align.Center);
    }

    protected static Rectangle GetSourceRectangle(int index)
    {
        var i = index % 3;
        var j = index / 3;
        return new Rectangle(i * 200, j * 200, 200, 200);
    }
}