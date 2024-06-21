using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace SpectatorMode.Framework;

internal class MenuTitle
{
    private readonly string text;
    private readonly Vector2 position;

    private readonly SpriteFont font = Game1.dialogueFont;

    public MenuTitle(string text)
    {
        this.text = text;
        position = new Vector2((Game1.uiViewport.Width - GetActualSize().X) / 2, 32);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, (int)position.X, (int)position.Y, (int)GetActualSize().X, (int)GetActualSize().Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, text, font, position + new Vector2(16, 16), Game1.textColor);
    }

    private Vector2 GetActualSize()
    {
        return font.MeasureString(text) + new Vector2(32, 32);
    }
}