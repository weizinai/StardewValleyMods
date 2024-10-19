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
                config => config.DefaultMenuTabId.ToString(),
                (config, value) => config.DefaultMenuTabId = Enum.Parse<MenuTabId>(value),
                I18n.Config_DefaultMenuTabID,
                null,
                new[] { "Favorite", "Farm", "Town", "Mountain", "Forest", "Beach", "Desert", "GingerIsland", "RSV", "SVE" },
                value =>
                {
                    var formatValue = value switch
                    {
                        "Favorite" => I18n.UI_Tab_Favorites(),
                        "Farm" => I18n.UI_Tab_Farm(),
                        "Town" => I18n.UI_Tab_Town(),
                        "Mountain" => I18n.UI_Tab_Mountain(),
                        "Forest" => I18n.UI_Tab_Forest(),
                        "Beach" => I18n.UI_Tab_Beach(),
                        "Desert" => I18n.UI_Tab_Desert(),
                        "GingerIsland" => I18n.UI_Tab_GingerIsland(),
                        "RSV" => I18n.UI_Tab_RSV(),
                        "SVE" => I18n.UI_Tab_SVE(),
                        _ => ""
                    };
                    return formatValue;
                }
            )
            .AddBoolOption(
                config => config.ProgressMode,
                (config, value) => config.ProgressMode = value,
                I18n.Config_ProgressMode_Name
            )
            .AddKeybindList(
                config => config.FavoriteKey,
                (config, value) => config.FavoriteKey = value,
                I18n.Config_FavoriteKey_Name
            );
    }
}