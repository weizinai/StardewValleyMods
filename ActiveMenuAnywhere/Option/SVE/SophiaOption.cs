using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class SophiaOption : BaseOption
{
    public SophiaOption()
        : base(I18n.UI_Option_Sophia(), TextureManager.Instance.SVETexture, GetSourceRectangle(0), OptionId.Sophia) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("FlashShifter.StardewValleyExpandedCP_SophiaLedger", "Sophia");
    }
}