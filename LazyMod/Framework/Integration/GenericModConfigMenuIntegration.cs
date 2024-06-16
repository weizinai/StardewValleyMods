using LazyMod;
using LazyMod.Framework.Config;

// ReSharper disable once CheckNamespace
namespace Common.Integrations;

/// <summary>Handles the other logic for integrating with the Generic Mod Config Menu mod.</summary>
/// <typeparam name="TConfig">The mod config type.</typeparam>
internal partial class GenericModConfigMenuIntegration<TConfig> where TConfig : new()
{
    public GenericModConfigMenuIntegration<TConfig> AddBaseAutomationConfig(Func<TConfig, BaseAutomationConfig> get, Func<string> text, Func<string>? tooltip, int minRange)
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
    
    public GenericModConfigMenuIntegration<TConfig> AddToolAutomationConfig(Func<TConfig, ToolAutomationConfig> get, Func<string> text, Func<string>? tooltip, int minRange)
    {
        AddBaseAutomationConfig(get, text, tooltip, minRange);
        AddBoolOption(
            config => get(config).FindToolFromInventory,
            (config, value) => get(config).FindToolFromInventory = value,
            I18n.Config_FindToolFromInventory_Name,
            I18n.Config_FindToolFromInventory_Tooltip
        );

        return this;
    }
    
    public GenericModConfigMenuIntegration<TConfig> AddStaminaToolAutomationConfig(Func<TConfig, StaminaToolAutomationConfig> get, 
        Func<string> text, Func<string>? tooltip, int minRange)
    {
        AddBaseAutomationConfig(get, text, tooltip, minRange);
        AddNumberOption(
            config => get(config).StopStamina,
            (config, value) => get(config).StopStamina = value,
            I18n.Config_StopStamina_Name
        );
        AddBoolOption(
            config => get(config).FindToolFromInventory,
            (config, value) => get(config).FindToolFromInventory = value,
            I18n.Config_FindToolFromInventory_Name,
            I18n.Config_FindToolFromInventory_Tooltip
        );

        return this;
    }
}