using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class PrizeTicketOption : BaseOption
{
    public PrizeTicketOption(Rectangle sourceRect) :
        base(I18n.Option_PrizeTicket(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Game1.activeClickableMenu = new PrizeTicketMenu();
    }
}