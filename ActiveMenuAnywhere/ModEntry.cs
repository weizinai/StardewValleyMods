using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        ModConfig.Init(helper);
        OptionFactory.Init(helper);
        TextureManager.Instance.LoadTexture(helper);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new Game1Patcher(helper));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            () => ModConfig.Instance,
            config => ModConfig.Instance = config,
            configMenu => configMenu
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
                )
        );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        var config = ModConfig.Instance;

        if (config.OpenMenuByTelephone)
        {
            return;
        }

        if (config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
            {
                Game1.exitActiveMenu();
            }
            else if (Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new AMAMenu(config.DefaultMenuTabId, this.Helper);
            }
        }
    }
}
