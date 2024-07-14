using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

public class ModConfig
{
    // 打开设置菜单
    public KeybindList OpenConfigMenuKey { get; set; } = new(SButton.R);

    // 购物限制
    public bool PurchaseLimit { get; set; } = true;
    public int DefaultPurchaseLimit { get; set; } = 2000;

    // 禁止购买背包
    public bool BanLargeBackpack { get; set; }
    public bool BanDeluxeBackpack { get; set; } = true;

    // 禁止升级房屋
    public bool[] BanHouseUpgrade { get; set; } = { false, true, true };

    // 自动设置IP连接
    public bool AutoSetIpConnection { get; set; } = true;
    public int EnableTime { get; set; } = 6;
    public int DisableTime { get; set; } = 20;

    // 显示玩家数量
    public bool ShowPlayerCount { get; set; } = true;

    // 显示提示
    public bool ShowTip { get; set; } = true;
    public string TipText { get; set; } = "粉丝联机档QQ群：232127142";

    // 踢出未准备玩家
    public bool KickUnreadyPlayer { get; set; } = true;
    public KeybindList KickUnreadyPlayerKey { get; set; } = new(SButton.F3);

    // 版本限制
    public bool VersionLimit { get; set; } = true;
}