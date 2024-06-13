using LazyMod;
using LazyMod.Framework.Config;

// ReSharper disable once CheckNamespace
namespace Common.Integrations;

/// <summary>Handles the other logic for integrating with the Generic Mod Config Menu mod.</summary>
/// <typeparam name="TConfig">The mod config type.</typeparam>
internal partial class GenericModConfigMenuIntegration<TConfig> where TConfig : new()
{
    public GenericModConfigMenuIntegration<TConfig> AddBaseAutomationConfig(Func<TConfig, BaseAutomationConfig> get, Func<string> text, Func<string>? tooltip, int minRange = 0)
    {
        AddSectionTitle(text, tooltip);
        AddBoolOption(
            config => get(config).IsEnable,
            (config, value) => get(config).IsEnable = value,
            I18n.Config_Enable_Name
        );
        AddNumberOption(
            config => get(config).Range,
            (config, value) => get(config).Range = value,
            I18n.Config_Range_Name,
            null,
            minRange,
            3
        );

        return this;
    }
}