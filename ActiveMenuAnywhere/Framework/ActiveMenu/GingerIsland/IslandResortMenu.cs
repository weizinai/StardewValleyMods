using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class IslandResortMenu : BaseActiveMenu
{
    public IslandResortMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.RequireLocation<IslandSouth>("IslandSouth").resortOpenToday.Value)
            Utility.TryOpenShopMenu("ResortBar", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}