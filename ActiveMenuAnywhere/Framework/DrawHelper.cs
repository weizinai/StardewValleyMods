using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using static weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Align;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal class DrawHelper
{
    public static void DrawShadow()
    {
        var spriteBatch = Game1.spriteBatch;
        spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
    }

    public static void DrawTitle(int x, int y, string text, Align align)
    {
        DrawTab(x, y, Game1.dialogueFont, text, align);
    }

    public static void DrawTab(int x, int y, SpriteFont font, string text, Align align, float alpha = 1)
    {
        var spriteBatch = Game1.spriteBatch;
        var (innerWidth, innerHeight) = font.MeasureString(text);
        var border = (x: 16, y: 8);
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