using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal class TextBox : ClickableComponent
{
    private readonly Point position;

    public TextBox(Point position, string name) : base(Rectangle.Empty, name)
    {
        this.position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var size = Game1.dialogueFont.MeasureString(this.name).ToPoint();

        this.bounds = new Rectangle(this.position, size + new Point(32, 32));
        IClickableMenu.drawTextureBox(spriteBatch, this.bounds.X, this.bounds.Y, this.bounds.Width, this.bounds.Height, Color.White);
        Utility.drawTextWithShadow(spriteBatch, this.name, Game1.dialogueFont, new Vector2(this.position.X + 16, this.position.Y + 16), Game1.textColor);
    }
}