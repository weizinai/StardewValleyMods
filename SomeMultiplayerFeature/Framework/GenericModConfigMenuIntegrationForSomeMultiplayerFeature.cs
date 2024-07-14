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
                I18n.Config_OpenConfigMenuKey_Name
            )
            // 冻结金钱
            .AddSectionTitle(I18n.Config_PurchaseLimit_Name)
            .AddBoolOption(
                config => config.PurchaseLimit,
                (config, value) => config.PurchaseLimit = value,
                I18n.Config_PurchaseLimit_Name
            )
            .AddNumberOption(
                config => config.DefaultPurchaseLimit,
                (config, value) => config.DefaultPurchaseLimit = value,
                I18n.Config_DefaultPurchaseLimit_Name
            )
            .AddBoolOption(
                config => config.BanLargeBackpack,
                (config, value) => config.BanLargeBackpack = value,
                I18n.Config_BanLargeBackpack_Name
            )
            .AddBoolOption(
                config => config.BanDeluxeBackpack,
                (config, value) => config.BanDeluxeBackpack = value,
                I18n.Config_BanDeluxeBackpack_Name
            )
            .AddBoolOption(
                config => config.BanHouseUpgrade[0],
                (config, value) => config.BanHouseUpgrade[0] = value,
                () => I18n.Config_BanHouseUpgrade_Name(1)
            )
            .AddBoolOption(
                config => config.BanHouseUpgrade[1],
                (config, value) => config.BanHouseUpgrade[1] = value,
                () => I18n.Config_BanHouseUpgrade_Name(2)
            )
            .AddBoolOption(
                config => config.BanHouseUpgrade[2],
                (config, value) => config.BanHouseUpgrade[2] = value,
                () => I18n.Config_BanHouseUpgrade_Name(3)
            )
            // 自动设置Ip连接
            .AddSectionTitle(I18n.Config_AutoSetIpConnection_Name)
            .AddBoolOption(
                config => config.AutoSetIpConnection,
                (config, value) => config.AutoSetIpConnection = value,
                I18n.Config_AutoSetIpConnection_Name
            )
            .AddNumberOption(
                config => config.EnableTime,
                (config, value) => config.EnableTime = value,
                I18n.Config_EnableTime_Name,
                null,
                6,
                26
            )
            .AddNumberOption(
                config => config.DisableTime,
                (config, value) => config.DisableTime = value,
                I18n.Config_DisableTime_Name,
                null,
                6,
                26
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
            .AddSectionTitle(I18n.Config_KickUnreadyPlayer_Name)
            .AddBoolOption(
                config => config.KickUnreadyPlayer,
                (config, value) => config.KickUnreadyPlayer = value,
                I18n.Config_KickUnreadyPlayer_Name,
                I18n.Config_KickUnreadyPlayer_Tooltip
            )
            .AddKeybindList(
                config => config.KickUnreadyPlayerKey,
                (config, value) => config.KickUnreadyPlayerKey = value,
                I18n.Config_KickUnreadyPlayerKey_Name
            )
            // 版本限制
            .AddSectionTitle(I18n.Config_VersionLimit_Name)
            .AddBoolOption(
                config => config.VersionLimit,
                (config, value) => config.VersionLimit = value,
                I18n.Config_VersionLimit_Name,
                I18n.Config_VersionLimit_Tooltip
            );
    }
}