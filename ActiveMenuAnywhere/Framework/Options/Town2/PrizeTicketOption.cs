using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class PrizeTicketOption : BaseOption
{
    public PrizeTicketOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_PrizeTicket())
    {
    }

    public override void ReceiveLeftClick()
    {
        Game1.activeClickableMenu = new StardewValley.Menus.PrizeTicketMenu();
    }
}