using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.TestMod.Framework;

internal class GenericModConfigMenuIntegrationForTestMod
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForTestMod(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        this.configMenu = configMenu;
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
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
            );
    }
}