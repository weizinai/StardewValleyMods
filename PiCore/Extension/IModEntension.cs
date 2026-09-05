using System;
using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.PiCore.Extension;

/// <summary>Provides utility methods for registering a config menu.</summary>
public static class IModExtension
{
    /// <summary>Register the config UI for this mod, and invoke the builder delegate to add its options.</summary>
    /// <typeparam name="TConfig">The config model type.</typeparam>
    /// <param name="mod">The mod for which to register a config UI.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="setConfig">Overwrite the current config model.</param>
    /// <param name="builder">Build the config UI options. This is only invoked if Generic Mod Config Menu is available.</param>
    /// <param name="onReset">Apply the config changes after they've been reset.</param>
    /// <param name="onSave">Apply the config changes after they've been saved.</param>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    public static GenericModConfigMenuIntegration<TConfig>? AddGenericModConfigMenu<TConfig>(
        this IMod mod,
        Func<TConfig> getConfig,
        Action<TConfig> setConfig,
        Action<GenericModConfigMenuIntegration<TConfig>> builder,
        Action? onReset = null,
        Action? onSave = null,
        bool titleScreenOnly = false
    ) where TConfig : class, new()
    {
        var api = new GenericModConfigMenuIntegration<TConfig>(mod.Helper.ModRegistry, mod.Monitor, mod.ModManifest, getConfig, Reset, Save);

        if (api.IsLoaded)
        {
            api.Register(titleScreenOnly);
            builder(api);

            return api;
        }

        return null;

        void Reset()
        {
            setConfig(new TConfig());
            onReset?.Invoke();
            mod.Helper.WriteConfig(getConfig());
        }

        void Save()
        {
            onSave?.Invoke();
            mod.Helper.WriteConfig(getConfig());
        }
    }
}
