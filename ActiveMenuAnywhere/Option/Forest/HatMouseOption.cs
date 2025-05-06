using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class HatMouseOption : BaseOption
{
    public HatMouseOption()
        : base(I18n.UI_Option_HatMouse(), TextureManager.Instance.ForestTexture, GetSourceRectangle(2), OptionId.HatMouse) { }

    public override bool IsEnable()
    {
        return Game1.player.achievements.Count > 0;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("HatMouse", null, true);
    }
}