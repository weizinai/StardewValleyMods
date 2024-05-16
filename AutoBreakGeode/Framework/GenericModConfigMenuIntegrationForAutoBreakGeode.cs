using Common.Integrations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AutoBreakGeode.Framework;

public class GenericModConfigMenuIntegrationForAutoBreakGeode
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForAutoBreakGeode(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (configMenu.GetConfig().OpenConfigMenuKeybind.JustPressed() && Context.IsPlayerFree)
            configMenu.OpenMenu();
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu
            .Register()
            .AddKeybindList(
                config => config.AutoBreakGeodeKey,
                (config, value) => config.AutoBreakGeodeKey = value,
                I18n.Config_AutoBreakGeodeKey_Name
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