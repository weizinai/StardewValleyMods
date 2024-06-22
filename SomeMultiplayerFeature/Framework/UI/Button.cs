using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework.UI;

internal class Button : ClickableComponent
{
    private readonly Point position;

    public Button(Point position, string name) : base(Rectangle.Empty, name)
    {
        this.position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var size = Game1.dialogueFont.MeasureString(name).ToPoint();

        bounds = new Rectangle(position, size + new Point(32, 32));
        IClickableMenu.drawTextureBox(spriteBatch, bounds.X, bounds.Y, bounds.Width, bounds.Height, Color.White);
        Utility.drawTextWithShadow(spriteBatch, name, Game1.dialogueFont, new Vector2(position.X + 16, position.Y + 16), Game1.textColor);
    }
}