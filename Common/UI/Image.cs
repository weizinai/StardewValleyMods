using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common.UI;

public class Image : Element, ISingleTexture
{
    /*********
     ** Accessors
     *********/
    /// <summary>The image texture to display.</summary>
    public Texture2D Texture { get; set; }

    /// <summary>The pixel area within the texture to display, or <c>null</c> to show the entire image.</summary>
    public Rectangle? TexturePixelArea { get; set; }

    /// <summary>The zoom factor to apply to the image.</summary>
    public int Scale { get; set; }

    public Action<Element> Callback { get; set; }

    /// <inheritdoc />
    public override int Width => (int)GetActualSize().X;

    /// <inheritdoc />
    public override int Height => (int)GetActualSize().Y;

    /// <inheritdoc />
    public override string HoveredSound => (Callback != null) ? "shiny4" : null;

    public Color DrawColor { get; set; } = Color.White;

    /*********
     ** Public methods
     *********/
    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        if (Clicked)
            Callback?.Invoke(this);
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        b.Draw(Texture, Position, TexturePixelArea, DrawColor, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
    }


    /*********
     ** Private methods
     *********/
    private Vector2 GetActualSize()
    {
        if (TexturePixelArea.HasValue)
            return new Vector2(TexturePixelArea.Value.Width, TexturePixelArea.Value.Height) * Scale;
        else
            return new Vector2(Texture.Width, Texture.Height) * Scale;
    }
}