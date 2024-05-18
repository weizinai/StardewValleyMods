using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

internal class VolcanoShopOption : BaseOption
{
    public VolcanoShopOption(Rectangle sourceRect) :
        base(I18n.Option_VolcanoShop(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("willyHours") && Game1.player.canUnderstandDwarves)
            Utility.TryOpenShopMenu("VolcanoShop", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}