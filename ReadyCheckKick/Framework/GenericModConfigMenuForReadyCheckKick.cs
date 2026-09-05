using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.ReadyCheckKick.Framework;

internal class GenericModConfigMenuForReadyCheckKick : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register()
            // Show unready farmers
            .AddSectionTitle(I18n.Config_ShowUnreadyFarmersTitle_Name)
            .AddBoolOption(
                config => config.ShowInfoInReadyCheckDialogue,
                (config, value) => config.ShowInfoInReadyCheckDialogue = value,
                I18n.Config_ShowInfoInReadyCheckDialogue_Name
            )
            .AddBoolOption(
                config => config.ShowInfoInSaveGameMenu,
                (config, value) => config.ShowInfoInSaveGameMenu = value,
                I18n.Config_ShowInfoInSaveGameMenu_Name
            )
            // Kick unready farmers
            .AddSectionTitle(I18n.Config_ShowUnreadyFarmersTitle_Name)
            .AddBoolOption(
                config => config.AutoKickUnreadyFarmers,
                (config, value) => config.AutoKickUnreadyFarmers = value,
                I18n.Config_AutoKickUnreadyFarmers_Name
            )
            .AddNumberOption(
                config => config.AutoKickUnreadyFarmersRatio,
                (config, value) => config.AutoKickUnreadyFarmersRatio = value,
                I18n.Config_AutoKickUnreadyFarmersRatio_Name,
                null,
                0f,
                1f,
                0.05f
            )
            .AddNumberOption(
                config => config.AutoKickUnreadyFarmersDelay,
                (config, value) => config.AutoKickUnreadyFarmersDelay = value,
                I18n.Config_AutoKickUnreadyFarmersDelay_Name
            );
    }
}