using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

public class ModConfig : SingletonConfig<ModConfig>
{
    // 打开设置菜单
    public KeybindList OpenConfigMenuKey { get; set; } = new(SButton.R);

    // 自动设置IP连接
    public bool AutoSetIpConnection { get; set; } = true;
    public int EnableTime { get; set; } = 6;
    public int DisableTime { get; set; } = 20;

    // 显示玩家数量
    public bool ShowPlayerCount { get; set; } = true;

    // 显示提示
    public bool ShowTip { get; set; } = true;
    public string TipText { get; set; } = "粉丝联机档QQ群：232127142";

    // 版本限制
    public bool VersionLimit { get; set; } = true;
    public int KickPlayerDelayTime { get; set; } = 10;
}
