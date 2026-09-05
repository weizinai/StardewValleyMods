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
    public bool ShowSpectateTooltip = true;
    public bool ShowTimeAndMoney = true;
    public bool ShowToolbar = true;

    // 旁观地点
    public KeybindList SpectateLocationKey = new(SButton.F6);
    public bool OnlyShowOutdoors = true;
    public int MoveSpeed = 32;
    public int MoveThreshold = 64;

    // 旁观玩家
    public KeybindList SpectatePlayerKey = new(SButton.F7);
    public KeybindList ToggleStateKey = new(SButton.K);
    public bool AutoSpectatePlayer;
    public int AutoSpectatePlayerTime = 800;

    // 随机旁观
    public KeybindList RandomSpectateKey = new(SButton.F8);
    public int RandomSpectateInterval = 30;
    public bool ShowRandomSpectateTooltip = true;

    // 自动睡觉
    public bool AutoSleep;
    public int AutoSleepTime = 2400;
    public bool SkipShippingMenu = true;

    // 自动节日
    public bool AutoParticipateFestival = true;

    // 自动事件
    public bool AutoSkipEvent = true;
}