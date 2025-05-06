﻿using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IslandTradeOption : BaseOption
{
    public IslandTradeOption()
        : base(I18n.UI_Option_IslandTrade(), TextureManager.Instance.GingerIslandTexture, GetSourceRectangle(3), OptionId.IslandTrade) { }

    public override bool IsEnable()
    {
        return Game1.RequireLocation<IslandNorth>("IslandNorth").traderActivated.Value;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("IslandTrade", null, true);
    }
}