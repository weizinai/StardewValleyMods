using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class JoiOption : BaseOption
{
    public JoiOption()
        : base(I18n.UI_Option_Joi(), TextureManager.Instance.RSVTexture, GetSourceRectangle(11), OptionId.Joi) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160254");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJioShop", null, false);
    }
}