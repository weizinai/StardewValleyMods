using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class LolaOption : BaseOption
{
    public LolaOption(Rectangle sourceRect) :
        base(I18n.UI_Option_Lola(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160093");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVLolaShop", "Lola");
    }
}