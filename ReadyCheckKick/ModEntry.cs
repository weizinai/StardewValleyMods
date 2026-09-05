using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.ReadyCheckKick.Framework;
using weizinai.StardewValleyMod.ReadyCheckKick.Handler;
using weizinai.StardewValleyMod.ReadyCheckKick.Patcher;

namespace weizinai.StardewValleyMod.ReadyCheckKick;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        I18n.Init(this.Helper.Translation);
        Logger<ModEntry>.Init(this);
        ModConfig.Init(helper);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        HarmonyPatcher.Apply(this, new SaveGameMenuPatcher(helper.Reflection));

        new ReadyCheckDialogueHandler(helper).Apply();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            () => ModConfig.Instance,
            config => ModConfig.Instance = config,
            configMenu => configMenu
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
                .AddSectionTitle(I18n.Config_KickUnreadyFarmersTitle_Name)
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
                )
        );
    }
}
