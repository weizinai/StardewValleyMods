using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class LolaOption : BaseOption
{
    public LolaOption(Rectangle sourceRect) :
        base(I18n.Option_Lola(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160093"))
            Utility.TryOpenShopMenu("RSVLolaShop", "Lola");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}