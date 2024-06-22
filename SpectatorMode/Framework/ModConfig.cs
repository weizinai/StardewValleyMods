using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class ModConfig
{
    // 旁观者模式
    public KeybindList SpectateLocationKeybind { get; set; } = new(SButton.F6);
    public KeybindList SpectatePlayerKeybind { get; set; } = new(SButton.F7);
    public KeybindList ToggleStateKeybind { get; set; } = new(SButton.K);
    public int MoveSpeed { get; set; } = 32;
    public int MoveThreshold { get; set; } = 64;

    // 轮播玩家
    public KeybindList RotatePlayerKeybind { get; set; } = new(SButton.F8);
    public int RotationInterval { get; set; } = 30;
}