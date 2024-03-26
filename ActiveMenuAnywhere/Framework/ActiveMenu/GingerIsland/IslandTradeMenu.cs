using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class IslandTradeMenu : BaseActiveMenu
{
    public IslandTradeMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.RequireLocation<IslandNorth>("IslandNorth").traderActivated.Value)
            Utility.TryOpenShopMenu("IslandTrade", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}