using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework;

public class Menu : IClickableMenu
{
    // private ClickableTextureComponent UpArrow;
    // private ClickableTextureComponent DownArrow;
    // private ClickableTextureComponent Scrollbar;
    /// <summary>The width of the borders drawn by <c>DrawTab</c>.</summary>
    public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

    private ClickableComponent _title;

    public Menu()
    {
        this.width = 800;
        this.height = 600;
        this.xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
        this.yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;
    }


    public override void draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
        Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
        this._title =
            new ClickableComponent(
                new Rectangle(this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen, Game1.tileSize * 4,
                    Game1.tileSize), "Menu");
        DrawTab(this._title.bounds.X, this._title.bounds.Y, Game1.dialogueFont, this._title.name, 1);
    }

    /// <summary>Draw a button texture fir the given text to the screen.</summary>
    /// <param name="x">The X position at which to draw.</param>
    /// <param name="y">The Y position at which to draw.</param>
    /// <param name="font">The text font.</param>
    /// <param name="text">The button text.</param>
    /// <param name="align">The button's horizontal alignment relative to <paramref name="x"/>. The possible values are 0 (left), 1 (center), or 2 (right).</param>
    /// <param name="alpha">The button opacity, as a value from 0 (transparent) to 1 (opaque).</param>
    /// <param name="drawShadow">Whether to draw a shadow under the tab.</param>
    public static void DrawTab(int x, int y, SpriteFont font, string text, int align = 0, float alpha = 1,
        bool drawShadow = true)
    {
        SpriteBatch spriteBatch = Game1.spriteBatch;
        Vector2 bounds = font.MeasureString(text);

        Menu.DrawTab(x, y, (int)bounds.X, (int)bounds.Y, out Vector2 drawPos, align, alpha, drawShadow: drawShadow);
        Utility.drawTextWithShadow(spriteBatch, text, font, drawPos, Game1.textColor);
    }

    /// <summary>Draw a button texture to the screen.</summary>
    /// <param name="x">The X position at which to draw.</param>
    /// <param name="y">The Y position at which to draw.</param>
    /// <param name="innerWidth">The width of the button's inner content.</param>
    /// <param name="innerHeight">The height of the button's inner content.</param>
    /// <param name="innerDrawPosition">The position at which the content should be drawn.</param>
    /// <param name="align">The button's horizontal alignment relative to <paramref name="x"/>. The possible values are 0 (left), 1 (center), or 2 (right).</param>
    /// <param name="alpha">The button opacity, as a value from 0 (transparent) to 1 (opaque).</param>
    /// <param name="drawShadow">Whether to draw a shadow under the tab.</param>
    public static void DrawTab(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition,
        int align = 0, float alpha = 1, bool drawShadow = true)
    {
        SpriteBatch spriteBatch = Game1.spriteBatch;

        // calculate outer coordinates
        int outerWidth = innerWidth + Menu.ButtonBorderWidth * 2;
        int outerHeight = innerHeight + Game1.tileSize / 3;
        int offsetX = align switch
        {
            1 => -outerWidth / 2,
            2 => -outerWidth,
            _ => 0
        };

        // draw texture
        IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y,
            outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
        innerDrawPosition = new Vector2(x + Menu.ButtonBorderWidth + offsetX, y + Menu.ButtonBorderWidth);
    }
}