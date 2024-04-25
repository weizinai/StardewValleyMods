using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Common.UI;

public class Image : Element
{
    private readonly Texture2D texture;

    private Rectangle localDestinationRectangle;

    private Rectangle DestinationRectangle
    {
        get
        {
            var location = localDestinationRectangle.Location.ToVector2() + (Parent?.Position ?? Vector2.Zero);
            return new Rectangle(location.ToPoint(), localDestinationRectangle.Size);
        }
    }
    private readonly Rectangle sourceRectangle;
    private readonly Color color;

    public override Vector2 LocalPosition
    {
        get => localDestinationRectangle.Location.ToVector2();
        set => localDestinationRectangle.Location = value.ToPoint();
    }

    protected override int Width => localDestinationRectangle.Width;
    protected override int Height => localDestinationRectangle.Height;

    private readonly bool isBackground;

    public Image(Texture2D texture, Rectangle localDestinationRectangle, Rectangle sourceRectangle, bool isBackground = false) :
        this(texture, localDestinationRectangle, sourceRectangle, Color.White, isBackground)
    {
    }

    public Image(Texture2D texture, Rectangle localDestinationRectangle, Rectangle sourceRectangle, Color color, bool isBackground = false)
    {
        this.texture = texture;
        this.localDestinationRectangle = localDestinationRectangle;
        this.sourceRectangle = sourceRectangle;
        this.color = color;
        this.isBackground = isBackground;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;

        if (isBackground)
            IClickableMenu.drawTextureBox(spriteBatch, texture, sourceRectangle, DestinationRectangle.X, DestinationRectangle.Y, 
                DestinationRectangle.Width, DestinationRectangle.Height, color, 1f, false);
        else
            spriteBatch.Draw(texture, DestinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
    }
}