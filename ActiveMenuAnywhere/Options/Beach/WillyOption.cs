using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class WillyOption : BaseOption
{
    public WillyOption(Rectangle sourceRect) :
        base(I18n.Option_Willy(), sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("spring_2_1"))
            Utility.TryOpenShopMenu("FishShop", "Willy");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}