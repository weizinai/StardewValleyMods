using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace FreeLock.Framework;

internal class ModConfig
{
    public KeybindList FreeLockKeybind { get; set; } = new(SButton.V);
}