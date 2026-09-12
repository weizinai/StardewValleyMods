using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.FreeLock.Framework;

internal class ModConfig : SingletonConfig<ModConfig>
{
    public KeybindList FreeLockKeybind { get; set; } = new(SButton.V);

    public int MoveSpeed { get; set; } = 32;
    public int MoveThreshold { get; set; } = 64;
}
