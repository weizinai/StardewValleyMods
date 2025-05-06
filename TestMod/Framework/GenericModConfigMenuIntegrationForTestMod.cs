using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.TestMod.Framework;

internal class GenericModConfigMenuIntegrationForTestMod : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register()
            .AddSectionTitle(I18n.Config_CardChance_Name)
            .AddBoolOption(
                config => config.CardChance.IsEnabled,
                (config, value) => config.CardChance.IsEnabled = value,
                I18n.Config_IsEnabled_Name
            )
            .AddNumberOption(
                config => config.CardChance.Value,
                (config, value) => config.CardChance.Value = value,
                I18n.Config_Value_Name
            )
            .AddSectionTitle(I18n.Config_WheelSpinSpeed_Name)
            .AddBoolOption(
                config => config.WheelSpinSpeed.IsEnabled,
                (config, value) => config.WheelSpinSpeed.IsEnabled = value,
                I18n.Config_IsEnabled_Name
            )
            .AddNumberOption(
                config => config.WheelSpinSpeed.Value,
                (config, value) => config.WheelSpinSpeed.Value = value,
                I18n.Config_Value_Name,
                null,
                0,
                14
            )
            .AddBoolOption(
                config => config.ExtraSpeed,
                (config, value) => config.ExtraSpeed = value,
                I18n.Config_ExtraSpeed_Name
            );
    }
}