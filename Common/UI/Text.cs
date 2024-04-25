using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public sealed class Text : Element
{
    public override Vector2 LocalPosition { get; set; }
    protected override int Width => (int)GetTextSize().X;
    protected override int Height => (int)GetTextSize().Y;

    private readonly SpriteFont font;
    private readonly string text;
    private readonly Color idleColor;
    private readonly Color hoverColor;
    private readonly float scale;

    public Text(string text, Vector2 localPosition, SpriteFont? font = null, Color? idleColor = null, Color? hoverColor = null, float scale = 1f)
    {
        this.font = font ?? Game1.dialogueFont;
        this.text = text;
        LocalPosition = localPosition;
        this.idleColor = idleColor ?? Game1.textColor;
        this.hoverColor = hoverColor ?? Game1.unselectedOptionColor;
        this.scale = scale;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;

        var altColor = Hover && OnHover is not null;
        var color = altColor ? hoverColor : idleColor;
        spriteBatch.DrawString(font, text, Position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    private Vector2 GetTextSize()
    {
        return font.MeasureString(text) * scale;
    }
}