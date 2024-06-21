using Common.Integrations;
using StardewModdingAPI;

namespace BetterCabin.Framework;

internal class GenericModConfigMenuIntegrationForBetterCabin
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForBetterCabin(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action save, Action reset)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, save, reset);
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu.Register();
    }
}