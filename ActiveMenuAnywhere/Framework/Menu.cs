using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Common;

namespace ActiveMenuAnywhere.Framework;

public class Menu : IClickableMenu
{
    // private ClickableTextureComponent UpArrow;
    // private ClickableTextureComponent DownArrow;
    // private ClickableTextureComponent Scrollbar;

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
        CommonHelper.DrawTab(this._title.bounds.X, this._title.bounds.Y, Game1.dialogueFont, this._title.name, 1);
    }

}