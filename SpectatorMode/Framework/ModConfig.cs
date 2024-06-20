using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SpectatorMode.Framework;

internal class ModConfig
{
    // 旁观者模式
    public KeybindList SpectatorModeKeybind { get; set; } = new(SButton.L);

    // 轮播玩家
    public KeybindList RotatePlayerKeybind { get; set; } = new(SButton.F8);
    public int RotationInterval { get; set; } = 30;
}