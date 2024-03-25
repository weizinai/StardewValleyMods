using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Beach;

public class BobberActiveMenu: BaseActiveMenu
{
    public BobberActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        Game1.activeClickableMenu = new ChooseFromIconsMenu("bobbers");
    }
}