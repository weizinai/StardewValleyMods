using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class ModConfig
{
    public static ModConfig Instance = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

    // 一般设置
    public bool ShowSpectateTooltip { get; set; } = true;
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
    public bool AutoSpectatePlayer { get; set; }
    public int AutoSpectatePlayerTime { get; set; } = 800;

    // 随机旁观
    public KeybindList RandomSpectateKey { get; set; } = new(SButton.F8);
    public int RandomSpectateInterval { get; set; } = 30;
    public bool ShowRandomSpectateTooltip { get; set; } = true;

    // 自动睡觉
    public bool AutoSleep { get; set; }
    public int AutoSleepTime { get; set; } = 2400;
    public bool SkipShippingMenu { get; set; } = true;

    // 自动节日
    public bool AutoParticipateFestival { get; set; } = true;

    // 自动事件
    public bool AutoSkipEvent { get; set; } = true;
}