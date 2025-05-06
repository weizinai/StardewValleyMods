﻿using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MagicBoatOption : BaseOption
{
    public MagicBoatOption()
        : base(I18n.UI_Option_MagicBoat(), TextureManager.Instance.BeachTexture, GetSourceRectangle(4), OptionId.MagicBoat) { }

    public override bool IsEnable()
    {
        return Utility.IsPassiveFestivalDay("NightMarket");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Festival_NightMarket_MagicBoat_Day" + Utility.GetDayOfPassiveFestival("NightMarket"), null, false);
    }
}