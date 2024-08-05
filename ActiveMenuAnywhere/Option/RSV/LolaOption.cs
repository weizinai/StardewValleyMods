using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class LolaOption : BaseOption
{
    public LolaOption() : base(I18n.UI_Option_Lola(), GetSourceRectangle(8)) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160093");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVLolaShop", "Lola");
    }
}