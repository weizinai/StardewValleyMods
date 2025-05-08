/*
代码来源：Pathoschild
原始出处：https://github.com/Pathoschild/StardewMods
授权协议：MIT License
*/

namespace weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

/// <summary>Implements the integration with Generic Mod Config Menu for a specific mod.</summary>
/// <typeparam name="TConfig">The config model type.</typeparam>
public interface IGenericModConfigMenuIntegrationFor<TConfig> where TConfig : class, new()
{
    /// <summary>Register the config UI for this mod. This should only be called if Generic Mod Config Menu is available.</summary>
    /// <param name="configMenu">The integration API through which to register the config menu.</param>
    public void Register(GenericModConfigMenuIntegration<TConfig> configMenu);
}