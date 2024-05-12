﻿using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class NightMarketTraveler : BaseOption
{
    public NightMarketTraveler(Rectangle sourceRect) : base("NightMarketTraveler", sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Utility.IsPassiveFestivalDay("NightMarket"))
            Utility.TryOpenShopMenu("Traveler", null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}