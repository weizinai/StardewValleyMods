using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ObserverMode.Framework;

internal class ModConfig
{
    public KeybindList ObserverModeKey { get; set; } = new(SButton.L);
}