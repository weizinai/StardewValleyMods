using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class WillyMenu : BaseActiveMenu
{
    public WillyMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("spring_2_1"))
            Utility.TryOpenShopMenu("FishShop", "Willy");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}