﻿using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DwarfOption : BaseOption
{
    public DwarfOption(Rectangle sourceRect) :
        base(I18n.Option_Dwarf(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.canUnderstandDwarves)
            Utility.TryOpenShopMenu("Dwarf", "Dwarf");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}