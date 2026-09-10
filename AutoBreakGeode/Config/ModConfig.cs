using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Config;

internal class ModConfig
{
    /// <summary>配置模块读到的配置实例（模组不持有副本）：处理器每帧/每次按键经它现读，故配置改动即时可见。</summary>
    public static ModConfig Instance { get; set; } = null!;

    /// <summary>切换自动砸晶球的快捷键；只在晶球菜单内生效，未手持晶球时按它不翻转开关。</summary>
    public KeybindList ToggleAutoBreakKeybind { get; set; } = new(SButton.F);

    /// <summary>
    /// 晶球动画的加速倍率（1 = 原版速度，默认取满）：语义与生效条件见 AutoBreakHandler.SpeedUpAnimation，
    /// 滑条上界取 AutoBreakHandler.MaxGeodeSpeed。
    /// </summary>
    public int BreakGeodeSpeed { get; set; } = 20;
}
