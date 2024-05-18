using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.Options;

internal class IslandResortOption : BaseOption
{
    public IslandResortOption(Rectangle sourceRect) :
        base(I18n.Option_IslandResort(), sourceRect)
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