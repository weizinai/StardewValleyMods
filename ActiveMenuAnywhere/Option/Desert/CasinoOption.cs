﻿using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class CasinoOption : BaseOption
{
    public CasinoOption(Rectangle sourceRect) :
        base(I18n.Option_Casino(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Casino", null, true);
    }
}