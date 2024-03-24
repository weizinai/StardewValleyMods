using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using static Common.Align;

namespace Common;

public static class DrawHelper
{
    /// <summary>The width of the borders drawn by <c>DrawTab</c>.</summary>
    public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

    /// <summary>Draw a button texture fir the given text to the screen.</summary>
    /// <param name="x">The X position at which to draw.</param>
    /// <param name="y">The Y position at which to draw.</param>
    /// <param name="font">The text font.</param>
    /// <param name="text">The button text.</param>
    /// <param name="align">
    ///     The button's horizontal alignment relative to <paramref name="x" />. The possible values are 0 (left), 1 (center), or 2
    ///     (right).
    /// </param>
    /// <param name="alpha">The button opacity, as a value from 0 (transparent) to 1 (opaque).</param>
    /// <param name="drawShadow">Whether to draw a shadow under the tab.</param>
    public static void DrawTab2(int x, int y, SpriteFont font, string text, int align = 0, float alpha = 1,
        bool drawShadow = true)
    {
        var spriteBatch = Game1.spriteBatch;
        var bounds = font.MeasureString(text);

        DrawTab2(x, y, (int)bounds.X, (int)bounds.Y, out var drawPos, align, alpha, drawShadow);
        Utility.drawTextWithShadow(spriteBatch, text, font, drawPos, Game1.textColor);
    }

    /// <summary>Draw a button texture to the screen.</summary>
    /// <param name="x">The X position at which to draw.</param>
    /// <param name="y">The Y position at which to draw.</param>
    /// <param name="innerWidth">The width of the button's inner content.</param>
    /// <param name="innerHeight">The height of the button's inner content.</param>
    /// <param name="innerDrawPosition">The position at which the content should be drawn.</param>
    /// <param name="align">
    ///     The button's horizontal alignment relative to <paramref name="x" />. The possible values are 0 (left), 1 (center), or 2
    ///     (right).
    /// </param>
    /// <param name="alpha">The button opacity, as a value from 0 (transparent) to 1 (opaque).</param>
    /// <param name="drawShadow">Whether to draw a shadow under the tab.</param>
    public static void DrawTab2(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition,
        int align = 0, float alpha = 1, bool drawShadow = true)
    {
        var spriteBatch = Game1.spriteBatch;

        // calculate outer coordinates
        var outerWidth = innerWidth + ButtonBorderWidth * 2;
        var outerHeight = innerHeight + Game1.tileSize / 3;
        var offsetX = align switch
        {
            1 => -outerWidth / 2,
            2 => -outerWidth,
            _ => 0
        };

        // draw texture
        IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y,
            outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
        innerDrawPosition = new Vector2(x + ButtonBorderWidth + offsetX, y + ButtonBorderWidth);
    }

    public static void DrawShadow()
    {
        var spriteBatch = Game1.spriteBatch;
        spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
    }

    public static void DrawTab(int x, int y, SpriteFont font, string text, Align align, float alpha = 1)
    {
        var spriteBatch = Game1.spriteBatch;
        var (innerWidth, innerHeight) = font.MeasureString(text);
        var border = (x: 1, y: 1);
        var outerWidth = (int)innerWidth + border.x * 2;
        var outerHeight = (int)innerHeight + border.y * 2;
        var offsetX = align switch
        {
            Left => 0,
            Center => -outerWidth / 2,
            Right => -outerWidth,
            _ => -outerWidth / 2
        };
        IClickableMenu.drawTextureBox(spriteBatch, x + offsetX, y, outerWidth, outerHeight, Color.White * alpha);
        var innerDrawPosition = new Vector2(x + offsetX + border.x, y + border.y);
        Utility.drawTextWithShadow(spriteBatch, text, font, innerDrawPosition, Game1.textColor);
    }
}

public enum Align
{
    Left,
    Center,
    Right
}