using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JericOption : BaseOption
{
    public JericOption()
        : base(I18n.UI_Option_Jeric(), TextureManager.Instance.RSVTexture, GetSourceRectangle(5), OptionId.Jeric) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJericShop", "Jeric");
    }
}