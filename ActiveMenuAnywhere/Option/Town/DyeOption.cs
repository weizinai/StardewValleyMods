﻿using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DyeOption : BaseOption
{
    public DyeOption()
        : base(I18n.UI_Option_Dye(), TextureManager.Instance.TownTexture, GetSourceRectangle(13), OptionId.Dye) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("992559");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new DyeMenu();
    }
}