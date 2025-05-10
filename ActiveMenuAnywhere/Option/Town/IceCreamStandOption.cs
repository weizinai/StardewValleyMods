using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class IceCreamStandOption : BaseOption
{
    public IceCreamStandOption()
        : base(I18n.UI_Option_IceCreamStand(), TextureManager.Instance.TownTexture, GetSourceRectangle(14), OptionId.IceCreamStand) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("IceCreamStand", null, true);
    }
}