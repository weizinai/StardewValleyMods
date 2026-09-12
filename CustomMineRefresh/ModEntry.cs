using StardewModdingAPI;
using weizinai.StardewValleyMod.CustomMineRefresh.Config;
using weizinai.StardewValleyMod.CustomMineRefresh.Handler;
using weizinai.StardewValleyMod.CustomMineRefresh.Policy;
using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.CustomMineRefresh;

internal class ModEntry : Mod
{
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        new ConfigService<ModConfig>(this).RegisterMenu(this.BuildConfigMenu);
        // 只有房主权威端（单人下也是）做判定，其他玩家什么都不用做、也不参与通信，故处理器按初始配置挂一次事件即可
        new RefreshHandler(helper).Apply();
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    /// <remarks>两组同构：各自的开关领出自己的分区，分区里跟着自己的节拍——关掉一组不会牵动另一组。</remarks>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddBoolSection(config => config.EnableMineRefresh, I18n.Config_MineRefresh_Name, I18n.Config_MineRefresh_Tooltip)
            .AddNumberOption(
                config => config.MineRefreshInterval,
                I18n.Config_MineRefreshInterval_Name,
                I18n.Config_MineRefreshInterval_Tooltip,
                min: 0, max: RefreshSchedule.MaxIntervalMinutes, interval: RefreshSchedule.ClockTickMinutes,
                formatValue: FormatIntervalValue
            )
            .AddBoolSection(config => config.EnableVolcanoRefresh, I18n.Config_VolcanoRefresh_Name, I18n.Config_VolcanoRefresh_Tooltip)
            .AddNumberOption(
                config => config.VolcanoRefreshInterval,
                I18n.Config_VolcanoRefreshInterval_Name,
                I18n.Config_VolcanoRefreshInterval_Tooltip,
                min: 0, max: RefreshSchedule.MaxIntervalMinutes, interval: RefreshSchedule.ClockTickMinutes,
                formatValue: FormatIntervalValue
            );
    }

    /// <summary>节拍值的显示文案：0 显示为「即时」，其余显示为「N 分钟」。</summary>
    /// <param name="minutes">配置里的节拍值（游戏分钟）。</param>
    private static string FormatIntervalValue(int minutes)
    {
        return RefreshSchedule.IsImmediate(minutes) ? I18n.Config_IntervalValue_Immediate() : I18n.Config_IntervalValue_Minutes(minutes);
    }
}
