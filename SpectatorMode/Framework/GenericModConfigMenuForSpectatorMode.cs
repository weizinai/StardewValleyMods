using Common.Integrations;
using StardewModdingAPI;

namespace SpectatorMode.Framework;

internal class GenericModConfigMenuForSpectatorMode
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuForSpectatorMode(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu
            .Register()
            .AddKeybindList(
                config => config.SpectatorModeKeybind,
                (config, value) => config.SpectatorModeKeybind = value,
                I18n.Config_SpectatorModeKeybind_Name
            )
            ;
    }
}