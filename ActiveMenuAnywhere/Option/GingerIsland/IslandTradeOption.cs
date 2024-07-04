using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IslandTradeOption : BaseOption
{
    public IslandTradeOption(Rectangle sourceRect) :
        base(I18n.Option_IslandTrade(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.RequireLocation<IslandNorth>("IslandNorth").traderActivated.Value;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("IslandTrade", null, true);
    }
}