using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MagicBoatOption : BaseOption
{
    public MagicBoatOption(Rectangle sourceRect) : base("MagicBoat", sourceRect) { }

    public override void ReceiveLeftClick()
    {
        if (Utility.IsPassiveFestivalDay("NightMarket"))
            Utility.TryOpenShopMenu("Festival_NightMarket_MagicBoat_Day" + Utility.GetDayOfPassiveFestival("NightMarket"), null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}