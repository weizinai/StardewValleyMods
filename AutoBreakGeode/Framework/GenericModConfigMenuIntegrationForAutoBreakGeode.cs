using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Framework;

internal class GenericModConfigMenuIntegrationForAutoBreakGeode
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForAutoBreakGeode(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.configMenu.GetConfig().OpenConfigMenuKeybind.JustPressed() && Context.IsPlayerFree) this.configMenu.OpenMenu();
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddKeybindList(
                config => config.OpenConfigMenuKeybind,
                (config, value) => config.OpenConfigMenuKeybind = value,
                I18n.Config_OpenConfigMenuKeybind_Name
            )
            .AddKeybindList(
                config => config.AutoBreakGeodeKeybind,
                (config, value) => config.AutoBreakGeodeKeybind = value,
                I18n.Config_AutoBreakGeodeKeybind_Name
            )
            .AddBoolOption(
                config => config.DrawBeginButton,
                (config, value) => config.DrawBeginButton = value,
                I18n.Config_DrawBeginButton_Name,
                I18n.Config_DrawBeginButton_Tooltip
            )
            .AddNumberOption(
                config => config.BreakGeodeSpeed,
                (config, value) => config.BreakGeodeSpeed = value,
                I18n.Config_BreakGeodeSpeed_Name
            );
    }
}