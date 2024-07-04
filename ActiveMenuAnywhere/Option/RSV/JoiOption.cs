using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JoiOption : BaseOption
{
    public JoiOption(Rectangle sourceRect) 
        : base(I18n.Option_Joi(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160254");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJioShop", null, false);
    }
}