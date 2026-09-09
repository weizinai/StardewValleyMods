using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class PrizeTicketOption : BaseOption
{
    public override void Apply()
    {
        Game1.activeClickableMenu = new PrizeTicketMenu();
    }
}
