using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class HarveyOption : BaseOption
{
    public HarveyOption()
        : base(I18n.UI_Option_Harvey(), TextureManager.Instance.TownTexture, GetSourceRectangle(11), OptionId.HarveyOption) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Hospital", "Harvey");
    }
}