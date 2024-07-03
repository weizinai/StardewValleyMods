using weizinai.StardewValleyMod.Common.Integration;

namespace MoreInfo.Framework;

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
            .Register();
    }
}