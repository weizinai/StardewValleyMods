using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.CustomMineRefresh.Config;

internal class ModConfig : SingletonConfig<ModConfig>
{
    /// <summary>是否刷新矿井里没有玩家的楼层。</summary>
    public bool EnableMineRefresh { get; set; } = true;

    /// <summary>矿井刷新节拍（游戏分钟）：0 表示即时（每秒检查一次）。</summary>
    public int MineRefreshInterval { get; set; }

    /// <summary>是否刷新火山地牢里没有玩家的楼层。</summary>
    public bool EnableVolcanoRefresh { get; set; } = true;

    /// <summary>火山刷新节拍（游戏分钟）：0 表示即时（每秒检查一次）。</summary>
    public int VolcanoRefreshInterval { get; set; } = 60;
}
