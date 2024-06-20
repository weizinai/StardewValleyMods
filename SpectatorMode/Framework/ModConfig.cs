using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SpectatorMode.Framework;

internal class ModConfig
{
    public KeybindList SpectatorModeKeybind { get; set; } = new(SButton.L);
}