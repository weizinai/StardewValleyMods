using Common.Integrations;
using StardewModdingAPI;

namespace FreeLock.Framework;

internal class GenericModConfigMenuForFreeLock
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuForFreeLock(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu
            .Register()
            .AddKeybindList(
                config => config.FreeLockKeybind,
                (config, value) => config.FreeLockKeybind = value,
                I18n.Config_FreeLockKeybind_Name
            )
            .AddNumberOption(
                config => config.MoveSpeed,
                (config, value) => config.MoveSpeed = value,
                I18n.Config_MoveSpeed_Name,
                I18n.Config_MoveSpeed_Tooltip
            )
            .AddNumberOption(
                config => config.MoveThreshold,
                (config, value) => config.MoveThreshold = value,
                I18n.Config_MoveThreshold_Name,
                I18n.Config_MoveThreshold_Tooltip
            )
            ;
    }
}