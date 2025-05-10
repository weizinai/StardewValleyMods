using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class VolcanoShopOption : BaseOption
{
    public VolcanoShopOption()
        : base(I18n.UI_Option_VolcanoShop(), TextureManager.Instance.GingerIslandTexture, GetSourceRectangle(5), OptionId.VolcanoShop) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("willyHours") && Game1.player.canUnderstandDwarves;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("VolcanoShop", null, true);
    }
}