using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SomeMultiplayerFeature.Framework;

public class ModConfig
{
    // 访问商店信息
    public bool ShowAccessShopInfo { get; set; } = true;
    
    // 踢出延迟玩家
    public bool EnableKickDelayedPlayer { get; set; } = true;
    
    // 模组限制
    public bool EnableModLimit { get; set; } = true;
    
    // 禁用金钱作弊
    public bool BanMoneyCheat { get; set; } = true;
    
    // 显示玩家数量
    public bool ShowPlayerCount { get; set; } = true;
    
    // 踢出未准备玩家
    public bool EnableKickUnreadyPlayer { get; set; } = true;
    public KeybindList KickUnreadyPlayerKey { get; set; } = new(SButton.F3);
}