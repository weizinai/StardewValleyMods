﻿using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class DecorationBoatOption : BaseOption
{
    public DecorationBoatOption(Rectangle sourceRect) : base("DecorationBoat", sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Utility.IsPassiveFestivalDay("NightMarket"))
            Utility.TryOpenShopMenu("Festival_NightMarket_DecorationBoat", null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}