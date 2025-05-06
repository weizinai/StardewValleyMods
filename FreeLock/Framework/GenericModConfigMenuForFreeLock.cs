using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.FreeLock.Framework;

internal class GenericModConfigMenuForFreeLock : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register()
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
            );
    }
}