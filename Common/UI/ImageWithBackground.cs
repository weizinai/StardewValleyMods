using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public class ImageWithBackground : Container
{
    private const int ContentPadding = 16;
    // private readonly Image background;
    // private readonly Image content;

    public Rectangle LocalDestinationRectangle;

    private Rectangle DestinationRectangle
    {
        get
        {
            var location = LocalDestinationRectangle.Location.ToVector2() + (Parent?.Position ?? Vector2.Zero);
            return new Rectangle(location.ToPoint(), LocalDestinationRectangle.Size);
        }
    }

    public override Vector2 LocalPosition
    {
        get => LocalDestinationRectangle.Location.ToVector2();
        set
        {
            LocalDestinationRectangle.Location = value.ToPoint();
            (Children[1] as Image)!.LocalDestinationRectangle = GetContentRectangle();
        }
    }

    public override int Width => DestinationRectangle.Width;
    public override int Height => DestinationRectangle.Height;

    public ImageWithBackground(Texture2D content, Rectangle contentRectangle, Rectangle localDestinationRectangle) :
        this(Game1.temporaryContent.Load<Texture2D>("Maps/MenuTiles"), new Rectangle(0, 256, 64, 64), Color.White,
            content, contentRectangle, Color.White, localDestinationRectangle)
    {
    }

    private ImageWithBackground(Texture2D background, Rectangle backgroundRectangle, Color backgroundColor,
        Texture2D content, Rectangle contentRectangle, Color contentColor, Rectangle localDestinationRectangle)
    {
        LocalDestinationRectangle = localDestinationRectangle;
        AddChild(new Image(background, Rectangle.Empty, backgroundRectangle, backgroundColor, true),
            new Image(content, GetContentRectangle(), contentRectangle, contentColor));
    }

    private Rectangle GetContentRectangle()
    {
        return new Rectangle(ContentPadding, ContentPadding, 
            LocalDestinationRectangle.Width - ContentPadding * 2, LocalDestinationRectangle.Height - ContentPadding * 2);
    }
}