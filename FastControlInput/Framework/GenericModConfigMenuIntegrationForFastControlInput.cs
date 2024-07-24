using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.FastControlInput.Framework;

internal class GenericModConfigMenuIntegrationForFastControlInput
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForFastControlInput(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        this.configMenu = configMenu;
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddNumberOption(
                config => config.ActionButton,
                (config, value) => config.ActionButton = value,
                I18n.Config_ActionButton_Name,
                I18n.Config_ActionButton_Tooltip,
                1f,
                5f,
                0.25f
            )
            .AddNumberOption(
                config => config.UseToolButton,
                (config, value) => config.UseToolButton = value,
                I18n.Config_UseToolButton_Name,
                I18n.Config_UseToolButton_Tooltip,
                1f,
                5f,
                0.25f
            );
    }
}