using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Beach;

public class WillyActiveMenu : BaseActiveMenu
{
    public WillyActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("FishShop", "Willy");
    }
}