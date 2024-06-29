using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class KimpoiOption : BaseOption
{
    public KimpoiOption(Rectangle sourceRect) :
        base(I18n.Option_Kimpoi(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.eventsSeen.Contains("75160252"))
            Utility.TryOpenShopMenu("RSVKimpoiShop", "Kimpoi");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}