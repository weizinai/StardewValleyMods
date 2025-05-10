using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class KimpoiOption : BaseOption
{
    public KimpoiOption()
        : base(I18n.UI_Option_Kimpoi(), TextureManager.Instance.RSVTexture, GetSourceRectangle(6), OptionId.Kimpoi) { }

    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("75160252");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVKimpoiShop", "Kimpoi");
    }
}