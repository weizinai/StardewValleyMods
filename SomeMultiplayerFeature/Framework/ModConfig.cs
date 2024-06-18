using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SomeMultiplayerFeature.Framework;

public class ModConfig
{
    public bool ShowAccessShopInfo { get; set; } = true;
    public bool EnableModLimit { get; set; } = true;
    public bool EnableKickDelayedPlayer { get; set; } = true;
    public bool ShowPlayerCount { get; set; } = true;
    public KeybindList KickUnreadyPlayerKey { get; set; } = new(SButton.F2);
    public bool BanMoneyCheat { get; set; } = true;
}