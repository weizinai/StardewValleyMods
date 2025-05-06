using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Framework;

internal class GenericModConfigMenuIntegrationForAutoBreakGeode : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register()
            .AddKeybindList(
                config => config.AutoBreakGeodeKeybind,
                (config, value) => config.AutoBreakGeodeKeybind = value,
                I18n.Config_AutoBreakGeodeKeybind_Name
            )
            .AddBoolOption(
                config => config.DrawBeginButton,
                (config, value) => config.DrawBeginButton = value,
                I18n.Config_DrawBeginButton_Name,
                I18n.Config_DrawBeginButton_Tooltip
            )
            .AddNumberOption(
                config => config.BreakGeodeSpeed,
                (config, value) => config.BreakGeodeSpeed = value,
                I18n.Config_BreakGeodeSpeed_Name
            );
    }
}