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
            // 旁观者模式
            .AddSectionTitle(I18n.Config_SpectatorModeTitle_Name)
            .AddKeybindList(
                config => config.SpectateLocationKeybind,
                (config, value) => config.SpectateLocationKeybind = value,
                I18n.Config_SpectateLocationKeybind_Name
            )
            .AddKeybindList(
                config => config.SpectatePlayerKeybind,
                (config, value) => config.SpectatePlayerKeybind = value,
                I18n.Config_SpectatePlayerKeybind_Name
            )
            .AddNumberOption(
                config => config.MoveSpeed,
                (config, value) => config.MoveSpeed = value,
                I18n.Config_MoveSpeed_Name
            )
            .AddNumberOption(
                config => config.MoveThreshold,
                (config, value) => config.MoveThreshold = value,
                I18n.Config_MoveThreshold_Name
            )
            // 轮播玩家
            .AddKeybindList(
                config => config.RotatePlayerKeybind,
                (config, value) => config.RotatePlayerKeybind = value,
                I18n.Config_RotatePlayerKeybind_Name
            )
            .AddNumberOption(
                config => config.RotationInterval,
                (config, value) => config.RotationInterval = value,
                I18n.Config_RotationInterval_Name
            )
            ;
    }
}