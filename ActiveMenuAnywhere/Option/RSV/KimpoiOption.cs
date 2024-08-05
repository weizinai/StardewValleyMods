using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class KimpoiOption : BaseOption
{
    public KimpoiOption() : base(I18n.UI_Option_Kimpoi(), GetSourceRectangle(6)) { }

    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("75160252");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVKimpoiShop", "Kimpoi");
    }
}