using System;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.LazyMod.Integration;

public static class GenericModConfigMenuIntegrationExtension
{
    public static GenericModConfigMenuIntegration<ModConfig> AddBaseAutomationConfig(
        this GenericModConfigMenuIntegration<ModConfig> configMenu,
        Func<ModConfig, BaseAutomationConfig> get,
        Func<string> text,
        Func<string>? tooltip,
        int minRange
    )
    {
        configMenu.AddSectionTitle(text, tooltip);
        configMenu.AddBoolOption(
            config => get(config).IsEnable,
            (config, value) => get(config).IsEnable = value,
            I18n.Config_Enable_Name
        );
        configMenu.AddNumberOption(
            config => get(config).Range,
            (config, value) => get(config).Range = value,
            I18n.Config_Range_Name,
            null,
            minRange,
            5
        );

        return configMenu;
    }

    public static GenericModConfigMenuIntegration<ModConfig> AddToolAutomationConfig(
        this GenericModConfigMenuIntegration<ModConfig> configMenu,
        Func<ModConfig, ToolAutomationConfig> get,
        Func<string> text,
        Func<string>? tooltip,
        int minRange
    )
    {
        configMenu.AddBaseAutomationConfig(get, text, tooltip, minRange);
        configMenu.AddBoolOption(
            config => get(config).FindToolFromInventory,
            (config, value) => get(config).FindToolFromInventory = value,
            I18n.Config_FindToolFromInventory_Name,
            I18n.Config_FindToolFromInventory_Tooltip
        );

        return configMenu;
    }

    public static GenericModConfigMenuIntegration<ModConfig> AddStaminaToolAutomationConfig(
        this GenericModConfigMenuIntegration<ModConfig> configMenu,
        Func<ModConfig, StaminaToolAutomationConfig> get,
        Func<string> text,
        Func<string>? tooltip,
        int minRange
    )
    {
        configMenu.AddBaseAutomationConfig(get, text, tooltip, minRange);
        configMenu.AddNumberOption(
            config => get(config).StopStamina,
            (config, value) => get(config).StopStamina = value,
            I18n.Config_StopStamina_Name
        );
        configMenu.AddBoolOption(
            config => get(config).FindToolFromInventory,
            (config, value) => get(config).FindToolFromInventory = value,
            I18n.Config_FindToolFromInventory_Name,
            I18n.Config_FindToolFromInventory_Tooltip
        );

        return configMenu;
    }
}