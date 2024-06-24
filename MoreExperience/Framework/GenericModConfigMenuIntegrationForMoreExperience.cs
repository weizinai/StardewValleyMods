using StardewModdingAPI;
using weizinai.StardewValleyMod.Common.Integration;

namespace MoreExperience.Framework;

internal class GenericModConfigMenuIntegrationForMoreExperience
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForMoreExperience(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;
        
        this.configMenu
            .Register();
    }
}