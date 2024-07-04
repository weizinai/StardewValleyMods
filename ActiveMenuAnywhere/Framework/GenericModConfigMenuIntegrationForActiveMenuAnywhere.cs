using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal class GenericModConfigMenuIntegrationForActiveMenuAnywhere
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForActiveMenuAnywhere(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        this.configMenu = configMenu;
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddKeybindList(
                config => config.MenuKey,
                (config, value) => config.MenuKey = value,
                I18n.Config_MenuKeyName
            )
            .AddBoolOption(
                config => config.OpenMenuByTelephone,
                (config, value) => config.OpenMenuByTelephone = value,
                I18n.Config_OpenMenuByTelephone_Name,
                I18n.Config_OpenMenuByTelephone_Tooltip
            )
            .AddTextOption(
                config => config.DefaultMeanTabId.ToString(),
                (config, value) => config.DefaultMeanTabId = Enum.Parse<MenuTabId>(value),
                I18n.Config_DefaultMenuTabID,
                null,
                new[] { "Farm", "Town", "Mountain", "Forest", "Beach", "Desert", "GingerIsland", "RSV", "SVE" },
                value =>
                {
                    var formatValue = value switch
                    {
                        "Farm" => I18n.Tab_Farm(),
                        "Town" => I18n.Tab_Town(),
                        "Mountain" => I18n.Tab_Mountain(),
                        "Forest" => I18n.Tab_Forest(),
                        "Beach" => I18n.Tab_Beach(),
                        "Desert" => I18n.Tab_Desert(),
                        "GingerIsland" => I18n.Tab_GingerIsland(),
                        "RSV" => I18n.Tab_RSV(),
                        "SVE" => I18n.Tab_SVE(),
                        _ => ""
                    };
                    return formatValue;
                }
            );
    }
}