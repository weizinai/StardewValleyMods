using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class KrobusMenu : BaseActiveMenu
{
    public KrobusMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.hasRustyKey)
            Utility.TryOpenShopMenu("ShadowShop", "Krobus");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}