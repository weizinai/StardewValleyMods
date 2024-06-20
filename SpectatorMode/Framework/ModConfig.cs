using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SpectatorMode.Framework;

internal class ModConfig
{
    public KeybindList SpectatorModeKeybind { get; set; } = new(SButton.L);

    public KeybindList RotatePlayerKeybind { get; set; } = new(SButton.F8);
    public int RotationInterval { get; set; } = 30;
}