using System;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

internal static class GenericModConfigMenuIntegrationExtension
{
    public static GenericModConfigMenuIntegration<ModConfig> AddBaseQuestConfig(
        this GenericModConfigMenuIntegration<ModConfig> configMenu,
        Func<ModConfig, BaseQuestConfig> get,
        Func<string> text
    )
    {
        configMenu
            .AddSectionTitle(text)
            .AddNumberOption(
                config => get(config).Weight,
                (config, value) => get(config).Weight = value,
                I18n.Config_QuestWeight_Name
            )
            .AddNumberOption(
                config => get(config).RewardMultiplier,
                (config, value) => get(config).RewardMultiplier = value,
                I18n.Config_QuestRewardMultiplier_Name,
                null,
                0.25f,
                5f,
                0.25f
            )
            .AddNumberOption(
                config => get(config).Days,
                (config, value) => get(config).Days = value,
                I18n.Config_QuestDays_Name,
                I18n.Config_QuestDays_Tooltip,
                0,
                10,
                1
            );

        return configMenu;
    }
}