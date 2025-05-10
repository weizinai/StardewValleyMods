using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class GusOption : BaseOption
{
    public GusOption()
        : base(I18n.UI_Option_Gus(), TextureManager.Instance.TownTexture, GetSourceRectangle(5), OptionId.Gus) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Saloon", "Gus");
    }
}