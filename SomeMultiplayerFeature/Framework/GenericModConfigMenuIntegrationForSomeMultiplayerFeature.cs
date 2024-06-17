using Common.Integrations;
using StardewModdingAPI;

namespace SomeMultiplayerFeature.Framework;

public class GenericModConfigMenuIntegrationForSomeMultiplayerFeature
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForSomeMultiplayerFeature(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu
            .Register()
            .AddBoolOption(
                config => config.ShowAccessShopInfo,
                (config, value) => config.ShowAccessShopInfo = value,
                I18n.Config_ShowAccessShopInfo_Name,
                I18n.Config_ShowAccessShopInfo_Tooltip
            )
            .AddBoolOption(
                config => config.EnableModLimit,
                (config, value) => config.EnableModLimit = value,
                I18n.Config_EnableModLimit_Name
            )
            .AddBoolOption(
                config => config.EnableKickDelayedPlayer,
                (config, value) => config.EnableKickDelayedPlayer = value,
                I18n.Config_EnableKickDelayedPlayer_Name,
                I18n.Config_EnableKickDelayedPlayer_Tooltip
            )
            .AddBoolOption(
                config => config.ShowPlayerCount,
                (config, value) => config.ShowPlayerCount = value,
                I18n.Config_ShowPlayerCount_Name
            )
            .AddKeybindList(
                config => config.KickUnreadyPlayerKey,
                (config, value) => config.KickUnreadyPlayerKey = value,
                I18n.Config_KickUnreadyPlayerKey_Name
                )
            ;
    }
}