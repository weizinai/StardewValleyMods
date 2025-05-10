using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class PrizeTicketOption : BaseOption
{
    public PrizeTicketOption()
        : base(I18n.UI_Option_PrizeTicket(), TextureManager.Instance.TownTexture, GetSourceRectangle(7), OptionId.PrizeTicket) { }

    public override void Apply()
    {
        Game1.activeClickableMenu = new PrizeTicketMenu();
    }
}