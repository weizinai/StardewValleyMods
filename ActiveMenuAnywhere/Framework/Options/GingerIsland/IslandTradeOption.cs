using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class IslandTradeOption : BaseOption
{
    public IslandTradeOption(Rectangle sourceRect) :
        base(I18n.Option_IslandTrade(), sourceRect)
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