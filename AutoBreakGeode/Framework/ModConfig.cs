using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AutoBreakGeode.Framework;

public class ModConfig
{
    public KeybindList AutoBreakGeodeKey { get; set; } = new(SButton.F);
    public bool DrawBeginButton { get; set; } = true;
    public int BreakGeodeSpeed { get; set; } = 20;
}