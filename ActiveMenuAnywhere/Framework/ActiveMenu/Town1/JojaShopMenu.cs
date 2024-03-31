using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class JojaShopMenu : BaseActiveMenu
{
    public JojaShopMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (!Game1.MasterPlayer.hasCompletedCommunityCenter())
            Utility.TryOpenShopMenu("Joja", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}