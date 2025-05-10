using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class LorenzoOption : BaseOption
{
    public LorenzoOption()
        : base(I18n.UI_Option_Lorenzo(), TextureManager.Instance.RSVTexture, GetSourceRectangle(4), OptionId.Lorenzo) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVHeapsStore", "Lorenzo");
    }
}