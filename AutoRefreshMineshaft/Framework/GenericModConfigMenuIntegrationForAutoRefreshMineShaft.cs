using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.AutoRefreshMineShaft.Framework;

internal class GenericModConfigMenuIntegrationForAutoRefreshMineShaft : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu
            .Register()
            .AddBoolOption(
                config => config.EnableMod,
                (config, value) => config.EnableMod = value,
                I18n.Config_EnableMod_Name
            );
    }
}