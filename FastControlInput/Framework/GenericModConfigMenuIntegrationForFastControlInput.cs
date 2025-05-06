using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.FastControlInput.Framework;

internal class GenericModConfigMenuIntegrationForFastControlInput : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register()
            .AddNumberOption(
                config => config.ActionButton,
                (config, value) => config.ActionButton = value,
                I18n.Config_ActionButton_Name,
                I18n.Config_ActionButton_Tooltip,
                1f,
                10f,
                0.25f
            )
            .AddNumberOption(
                config => config.UseToolButton,
                (config, value) => config.UseToolButton = value,
                I18n.Config_UseToolButton_Name,
                I18n.Config_UseToolButton_Tooltip,
                1f,
                10f,
                0.25f
            );
    }
}