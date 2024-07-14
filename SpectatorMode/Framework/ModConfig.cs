using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class ModConfig
{
    // 一般设置
    public bool ShowTimeAndMoney { get; set; } = true;
    public bool ShowToolbar { get; set; } = true;

    // 旁观地点
    public KeybindList SpectateLocationKey { get; set; } = new(SButton.F6);
    public bool OnlyShowOutdoors { get; set; } = true;
    public int MoveSpeed { get; set; } = 32;
    public int MoveThreshold { get; set; } = 64;

    // 旁观玩家
    public KeybindList SpectatePlayerKey { get; set; } = new(SButton.F7);
    public KeybindList ToggleStateKey { get; set; } = new(SButton.K);

    // 随机旁观
    public KeybindList RandomSpectateKey { get; set; } = new(SButton.F8);
    public int RandomSpectateInterval { get; set; } = 30;
}