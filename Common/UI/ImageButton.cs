using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Common.UI;

public class ImageButton : Element
{
    private const int ContentPadding = 16;
    private readonly Texture2D background;
    private readonly Rectangle backgroundSourceRectangle;
    private readonly Color backgroundColor;
    private readonly Texture2D content;
    private readonly Rectangle contentSourceRectangle;
    private readonly Color contentColor;
    private readonly float scale;

    public ImageButton(Texture2D content, Rectangle contentSourceRectangle, Vector2 localPosition, float scale = 1f) :
        this(Game1.temporaryContent.Load<Texture2D>("Maps/MenuTiles"), new Rectangle(0, 256, 60, 60), Color.White,
            content, contentSourceRectangle, Color.White, localPosition, scale)
    {
    }

    private ImageButton(Texture2D background, Rectangle backgroundSourceRectangle, Color backgroundColor,
        Texture2D content, Rectangle contentSourceRectangle, Color contentColor, Vector2 localPosition, float scale = 1f)
    {
        this.background = background;
        this.backgroundSourceRectangle = backgroundSourceRectangle;
        this.backgroundColor = backgroundColor;
        this.content = content;
        this.contentSourceRectangle = contentSourceRectangle;
        this.contentColor = contentColor;
        LocalPosition = localPosition;
        this.scale = scale;
    }

    protected override int Width => (int)GetImageSize().X;
    protected override int Height => (int)GetImageSize().Y;


    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        IClickableMenu.drawTextureBox(spriteBatch, background, backgroundSourceRectangle,
            (int)Position.X, (int)Position.Y, Width, Height, backgroundColor, 1f, false);
        spriteBatch.Draw(content, GetContentRectangle(), contentSourceRectangle, contentColor);
    }

    private Rectangle GetContentRectangle()
    {
        return new Rectangle((int)Position.X + ContentPadding, (int)Position.Y + ContentPadding, Width - ContentPadding * 2, Height - ContentPadding * 2);
    }

    private Vector2 GetImageSize()
    {
        return new Vector2(backgroundSourceRectangle.Width, backgroundSourceRectangle.Height) * scale;
    }
}