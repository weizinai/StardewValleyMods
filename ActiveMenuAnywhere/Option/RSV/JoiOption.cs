using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JoiOption : BaseOption
{
    public JoiOption() : base(I18n.UI_Option_Joi(), GetSourceRectangle(11)) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160254");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJioShop", null, false);
    }
}