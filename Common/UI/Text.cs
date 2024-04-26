using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public sealed class Text : Element
{
    protected override int Width => (int)GetTextSize().X;
    protected override int Height => (int)GetTextSize().Y;

    private readonly SpriteFont font;
    private readonly string text;
    private readonly Color color;
    private readonly float scale;

    public Text(string text, Vector2 localPosition, SpriteFont? font = null, Color? color = null, float scale = 1f)
    {
        // this.font = font ?? Game1.temporaryContent.Load<SpriteFont>("Fonts/SpriteFont1");
        this.font = font ?? Game1.dialogueFont;
        this.text = text;
        LocalPosition = localPosition;
        this.color = color ?? Game1.textColor;
        this.scale = scale;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        
        
        spriteBatch.DrawString(font, text, Position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    private Vector2 GetTextSize()
    {
        return font.MeasureString(text) * scale;
    }
}