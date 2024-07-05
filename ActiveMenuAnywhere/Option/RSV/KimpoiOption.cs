using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class KimpoiOption : BaseOption
{
    public KimpoiOption(Rectangle sourceRect) :
        base(I18n.UI_Option_Kimpoi(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("75160252");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVKimpoiShop", "Kimpoi");
    }
}