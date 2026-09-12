using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Config;

internal class ModConfig : SingletonConfig<ModConfig>
{
    /// <summary>切换自动砸晶球的快捷键；只在晶球菜单内生效，未手持晶球时按它不翻转开关。</summary>
    public KeybindList ToggleAutoBreakKeybind { get; set; } = new(SButton.F);

    /// <summary>
    /// 晶球动画的加速倍率（1 = 原版速度，默认取满）：语义与生效条件见 GeodeSpeedPolicy，
    /// 滑条上界取 GeodeSpeedPolicy.MaxSpeed。
    /// </summary>
    public int BreakGeodeSpeed { get; set; } = 20;
}
