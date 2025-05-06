using StardewValley;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.CustomMachineExperience.Framework;

internal class GenericModConfigMenuIntegrationForMoreExperience : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register();

        foreach (var (id, _) in ModConfig.Instance.MachineExperienceData)
        {
            configMenu
                .AddSectionTitle(() => ItemRegistry.GetData(id).DisplayName)
                .AddNumberOption(
                    config => config.MachineExperienceData[id].FarmingExperience,
                    (config, value) => config.MachineExperienceData[id].FarmingExperience = value,
                    I18n.Config_FarmingSkill_Name
                )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].FishingExperience,
                    (config, value) => config.MachineExperienceData[id].FishingExperience = value,
                    I18n.Config_FishingSkill_Name
                )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].ForagingExperience,
                    (config, value) => config.MachineExperienceData[id].ForagingExperience = value,
                    I18n.Config_ForagingSkill_Name
                )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].MiningExperience,
                    (config, value) => config.MachineExperienceData[id].MiningExperience = value,
                    I18n.Config_MiningSkill_Name
                )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].CombatExperience,
                    (config, value) => config.MachineExperienceData[id].CombatExperience = value,
                    I18n.Config_CombatSkill_Name
                );
        }
    }
}