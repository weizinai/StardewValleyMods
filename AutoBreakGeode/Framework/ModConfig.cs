﻿using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Framework;

internal class ModConfig
{
    public KeybindList AutoBreakGeodeKeybind { get; set; } = new(SButton.F);
    public bool DrawBeginButton { get; set; } = true;
    public int BreakGeodeSpeed { get; set; } = 20;
}