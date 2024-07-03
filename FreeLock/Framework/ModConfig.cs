using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.FreeLock.Framework;

internal class ModConfig
{
    public KeybindList FreeLockKeybind { get; set; } = new(SButton.V);

    public int MoveSpeed { get; set; } = 32;
    public int MoveThreshold { get; set; } = 64;
}