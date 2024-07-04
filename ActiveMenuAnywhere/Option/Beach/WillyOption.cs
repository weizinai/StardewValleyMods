using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class WillyOption : BaseOption
{
    public WillyOption(Rectangle sourceRect) :
        base(I18n.Option_Willy(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("spring_2_1");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("FishShop", "Willy");
    }
}