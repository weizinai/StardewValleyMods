using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class PrizeTicketOption : BaseOption
{
    public PrizeTicketOption() : base(I18n.UI_Option_PrizeTicket(), GetSourceRectangle(7)) { }

    public override void Apply()
    {
        Game1.activeClickableMenu = new PrizeTicketMenu();
    }
}