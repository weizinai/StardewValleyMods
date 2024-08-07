using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.MoreInfo.Framework;

internal class GenericModConfigMenuIntegrationForMoreInfo
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForMoreInfo(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        this.configMenu = configMenu;
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddBoolOption(
                config => config.ShowObjectInfo,
                (config, value) => config.ShowObjectInfo = value,
                I18n.Config_ShowObjectInfo_Name
            )
            .AddBoolOption(
                config => config.ShowMonsterInfo,
                (config, value) => config.ShowMonsterInfo = value,
                I18n.Config_ShowMonsterInfo_Name
            );
    }
}