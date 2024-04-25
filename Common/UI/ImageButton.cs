using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public class ImageButton : Container
{
    private const int ContentPadding = 16;

    private Rectangle localDestinationRectangle;

    private Rectangle DestinationRectangle
    {
        get
        {
            var location = localDestinationRectangle.Location.ToVector2() + (Parent?.Position ?? Vector2.Zero);
            return new Rectangle(location.ToPoint(), localDestinationRectangle.Size);
        }
    }

    public override Vector2 LocalPosition
    {
        get => localDestinationRectangle.Location.ToVector2();
        set => localDestinationRectangle.Location = value.ToPoint();
    }

    public override int Width => DestinationRectangle.Width;
    public override int Height => DestinationRectangle.Height;

    public ImageButton(Texture2D content, Rectangle contentRectangle, Rectangle localDestinationRectangle) :
        this(Game1.temporaryContent.Load<Texture2D>("Maps/MenuTiles"), new Rectangle(0, 256, 64, 64), Color.White,
            content, contentRectangle, Color.White, localDestinationRectangle)
    {
    }

    private ImageButton(Texture2D background, Rectangle backgroundRectangle, Color backgroundColor,
        Texture2D content, Rectangle contentRectangle, Color contentColor, Rectangle localDestinationRectangle)
    {
        UpdateChildren = false;
        this.localDestinationRectangle = localDestinationRectangle;
        AddChild(new Image(background, new Rectangle(0,0,64,64), backgroundRectangle, backgroundColor, true),
            new Image(content, GetContentRectangle(), contentRectangle, contentColor));
    }

    public override void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        OnHover?.Invoke(spriteBatch);
    }

    private Rectangle GetContentRectangle()
    {
        return new Rectangle(ContentPadding, ContentPadding, 
            localDestinationRectangle.Width - ContentPadding * 2, localDestinationRectangle.Height - ContentPadding * 2);
    }
}