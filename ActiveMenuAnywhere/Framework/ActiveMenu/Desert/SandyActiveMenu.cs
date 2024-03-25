using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Desert;

public class SandyActiveMenu : BaseActiveMenu
{
    public SandyActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("Sandy", "Sandy");
    }
}