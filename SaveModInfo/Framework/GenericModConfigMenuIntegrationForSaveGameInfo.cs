using weizinai.StardewValleyMod.Common.Integration;

namespace SaveModInfo.Framework;

internal class GenericModConfigMenuIntegrationForSaveGameInfo
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;
    
    public GenericModConfigMenuIntegrationForSaveGameInfo(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        this.configMenu = configMenu;
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu.Register();
    }
}