using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class KrobusOption : BaseOption
{
    public KrobusOption()
        : base(I18n.UI_Option_Krobus(), TextureManager.Instance.TownTexture, GetSourceRectangle(9), OptionId.Krobus) { }

    public override bool IsEnable()
    {
        return Game1.player.hasRustyKey;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("ShadowShop", "Krobus");
    }
}