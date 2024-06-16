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
                config => config.AccessShopInfo,
                (config, value) => config.AccessShopInfo = value,
                I18n.Config_AccessShopInfo_Name,
                I18n.Config_AccessShopInfo_Tooltip
            )
            .AddBoolOption(
                config => config.EnableModLimit,
                (config, value) => config.EnableModLimit = value,
                I18n.Config_EnableModLimit_Name
            );
    }
}