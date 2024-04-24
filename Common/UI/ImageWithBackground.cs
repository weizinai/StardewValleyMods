using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common.UI;

public class ImageWithBackground : Element
{
    private const int ContentPadding = 16;
    private readonly Image background;
    private readonly Image content;
    
    public override Vector2 Position
    {
        get => background.DestinationRectangle.Location.ToVector2();
        set
        {
            background.DestinationRectangle.Location = value.ToPoint();
            content.DestinationRectangle = GetContentRectangle();
        }
    }

    public override int Width => background.Width;
    public override int Height => background.Height;

    public ImageWithBackground(Image background, Texture2D texture, Rectangle sourceRectangle) : this(background, texture, sourceRectangle, Color.White) { }

    private ImageWithBackground(Image background, Texture2D texture, Rectangle sourceRectangle, Color color)
    {
        this.background = background;
        content = new Image(texture, GetContentRectangle(), sourceRectangle, color);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        
        background.Draw(spriteBatch);
        content.Draw(spriteBatch);
    }

    private Rectangle GetContentRectangle()
    {
        return new Rectangle(background.DestinationRectangle.X + ContentPadding, background.DestinationRectangle.Y + ContentPadding,
            background.DestinationRectangle.Width - ContentPadding * 2, background.DestinationRectangle.Height - ContentPadding * 2);
    }
}