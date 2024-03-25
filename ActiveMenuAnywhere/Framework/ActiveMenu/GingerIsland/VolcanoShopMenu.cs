using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class VolcanoShopMenu: BaseActiveMenu
{
    public VolcanoShopMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("willyHours") && Game1.player.canUnderstandDwarves)
            Utility.TryOpenShopMenu("VolcanoShop", null, playOpenSound: true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}