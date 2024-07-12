using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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

        this.configMenu.Unregister();

        this.configMenu
            .Register()
            .AddKeybindList(
                config => config.OpenConfigMenuKey,
                (config, value) => config.OpenConfigMenuKey = value,
                I18n.Config_OpenConfigMenuKey_Name
            )
            // 冻结金钱
            .AddSectionTitle(I18n.Config_FreezeMoney_Name, enable: Game1.IsServer)
            .AddBoolOption(
                config => config.FreezeMoney,
                (config, value) => config.FreezeMoney = value,
                I18n.Config_FreezeMoney_Name
            )
            .AddNumberOption(
                config => config.FreezeMoneyAmount,
                (config, value) => config.FreezeMoneyAmount = value,
                I18n.Config_FreezeMoneyAmount_Name
            )
            // 自动设置Ip连接
            .AddSectionTitle(I18n.Config_AutoSetIpConnection_Name, enable: Game1.IsServer)
            .AddBoolOption(
                config => config.AutoSetIpConnection,
                (config, value) => config.AutoSetIpConnection = value,
                I18n.Config_AutoSetIpConnection_Name,
                enable: Game1.IsServer
            )
            .AddNumberOption(
                config => config.EnableTime,
                (config, value) => config.EnableTime = value,
                I18n.Config_EnableTime_Name,
                null,
                6,
                26,
                enable: Game1.IsServer
            )
            .AddNumberOption(
                config => config.DisableTime,
                (config, value) => config.DisableTime = value,
                I18n.Config_DisableTime_Name,
                null,
                6,
                26,
                enable: Game1.IsServer
            )
            // 显示玩家数量
            .AddSectionTitle(I18n.Config_ShowPlayerCount_Name)
            .AddBoolOption(
                config => config.ShowPlayerCount,
                (config, value) => config.ShowPlayerCount = value,
                I18n.Config_ShowPlayerCount_Name,
                I18n.Config_ShowPlayerCount_Tooltip
            )
            // 显示提示
            .AddSectionTitle(I18n.Config_ShowTip_Name)
            .AddBoolOption(
                config => config.ShowTip,
                (config, value) => config.ShowTip = value,
                I18n.Config_ShowTip_Name,
                I18n.Config_ShowTip_Tooltip
            )
            .AddTextOption(
                config => config.TipText,
                (config, value) => config.TipText = value,
                I18n.Config_TipText_Name
            )
            // 踢出未准备玩家
            .AddSectionTitle(I18n.Config_KickUnreadyPlayer_Name, enable: Game1.IsServer)
            .AddBoolOption(
                config => config.KickUnreadyPlayer,
                (config, value) => config.KickUnreadyPlayer = value,
                I18n.Config_KickUnreadyPlayer_Name,
                I18n.Config_KickUnreadyPlayer_Tooltip,
                enable: Game1.IsServer
            )
            .AddKeybindList(
                config => config.KickUnreadyPlayerKey,
                (config, value) => config.KickUnreadyPlayerKey = value,
                I18n.Config_KickUnreadyPlayerKey_Name,
                enable: Game1.IsServer
            )
            // 版本限制
            .AddSectionTitle(I18n.Config_VersionLimit_Name)
            .AddBoolOption(
                config => config.VersionLimit,
                (config, value) => config.VersionLimit = value,
                I18n.Config_VersionLimit_Name,
                I18n.Config_VersionLimit_Tooltip,
                enable: Game1.IsServer
            );
    }
}