using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Common.UI;

public class Image : Element
{
    private readonly Texture2D texture;
    public Rectangle DestinationRectangle;
    private readonly Rectangle sourceRectangle;
    private readonly Color color;

    public override Vector2 Position
    {
        get => DestinationRectangle.Location.ToVector2();
        set => DestinationRectangle.Location = value.ToPoint();
    }

    public override int Width => DestinationRectangle.Width;
    public override int Height => DestinationRectangle.Height;

    private readonly bool isBackground;

    public Image(Texture2D texture, Rectangle destinationRectangle, Rectangle sourceRectangle, bool isBackground = false) :
        this(texture, destinationRectangle, sourceRectangle, Color.White, isBackground)
    {
    }

    public Image(Texture2D texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, bool isBackground = false)
    {
        this.texture = texture;
        DestinationRectangle = destinationRectangle;
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