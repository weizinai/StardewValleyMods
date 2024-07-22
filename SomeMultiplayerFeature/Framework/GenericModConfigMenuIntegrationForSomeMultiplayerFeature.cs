using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal class GenericModConfigMenuIntegrationForSomeMultiplayerFeature
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForSomeMultiplayerFeature(GenericModConfigMenuIntegration<ModConfig> configMenu, IInputEvents inputEvents)
    {
        this.configMenu = configMenu;
        inputEvents.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.configMenu.GetConfig().OpenConfigMenuKey.JustPressed())
            this.configMenu.OpenMenu();
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddKeybindList(
                config => config.OpenConfigMenuKey,
                (config, value) => config.OpenConfigMenuKey = value,
                () => "打开配置菜单快捷键"
            )
            // 花钱限制
            .AddSectionTitle(() => "花钱限制")
            .AddBoolOption(
                config => config.SpendLimit,
                (config, value) => config.SpendLimit = value,
                () => "花钱限制"
            )
            .AddNumberOption(
                config => config.DefaultSpendLimit,
                (config, value) => config.DefaultSpendLimit = value,
                () => "默认花钱额度"
            )
            .AddKeybindList(
                config => config.SpendLimitManagerMenuKey,
                (config, value) => config.SpendLimitManagerMenuKey = value,
                () => "花钱限制管理快捷键"
            )
            // 自动设置Ip连接
            .AddSectionTitle(() => "自动设置Ip连接")
            .AddBoolOption(
                config => config.AutoSetIpConnection,
                (config, value) => config.AutoSetIpConnection = value,
                () => "自动设置Ip连接"
            )
            .AddNumberOption(
                config => config.EnableTime,
                (config, value) => config.EnableTime = value,
                () => "启用时间",
                null,
                6,
                26
            )
            .AddNumberOption(
                config => config.DisableTime,
                (config, value) => config.DisableTime = value,
                () => "禁用时间",
                null,
                6,
                26
            )
            // 显示玩家数量
            .AddSectionTitle(() => "显示玩家数量")
            .AddBoolOption(
                config => config.ShowPlayerCount,
                (config, value) => config.ShowPlayerCount = value,
                () => "显示玩家数量"
            )
            // 显示提示
            .AddSectionTitle(() => "显示提示")
            .AddBoolOption(
                config => config.ShowTip,
                (config, value) => config.ShowTip = value,
                () => "显示提示"
            )
            .AddTextOption(
                config => config.TipText,
                (config, value) => config.TipText = value,
                () => "提示内容"
            )
            // 踢出未准备玩家
            .AddSectionTitle(() => "踢出未准备玩家")
            .AddBoolOption(
                config => config.KickUnreadyPlayer,
                (config, value) => config.KickUnreadyPlayer = value,
                () => "踢出未准备玩家"
            )
            .AddKeybindList(
                config => config.KickUnreadyPlayerKey,
                (config, value) => config.KickUnreadyPlayerKey = value,
                () => "踢出未准备玩家快捷键"
            )
            // 版本限制
            .AddSectionTitle(() => "版本限制")
            .AddBoolOption(
                config => config.VersionLimit,
                (config, value) => config.VersionLimit = value,
                () => "版本限制"
            );
    }
}