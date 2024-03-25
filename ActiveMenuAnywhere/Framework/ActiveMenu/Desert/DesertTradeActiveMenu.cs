using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Desert;

public class DesertTradeActiveMenu : BaseActiveMenu
{
    public DesertTradeActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("DesertTrade", "DesertTrade");
    }
}