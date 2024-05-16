using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AutoBreakGeode.Framework;

internal class ModConfig
{
    public KeybindList OpenConfigMenuKeybind = new(SButton.None);
    public KeybindList AutoBreakGeodeKey = new(SButton.F);
    public bool DrawBeginButton = true;
    public int BreakGeodeSpeed= 20;
}