using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.FreeLock.Framework;
using weizinai.StardewValleyMod.PiCore;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.FreeLock;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Player.Warped += this.OnWarped;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || !Game1.viewportFreeze)
        {
            return;
        }

        PanScreenHelper.PanScreenByMouse(this.config.MoveSpeed, this.config.MoveThreshold);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree)
        {
            return;
        }

        if (this.config.FreeLockKeybind.JustPressed())
        {
            Game1.viewportFreeze = !Game1.viewportFreeze;
            HudLogger.NoIconHUDMessage(Game1.viewportFreeze ? I18n.UI_ViewportUnlocked_Tooltip() : I18n.UI_ViewportLocked_Tooltip(), 1000f);
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (Game1.viewportFreeze)
        {
            Game1.viewportFreeze = false;
            HudLogger.NoIconHUDMessage(I18n.UI_ViewportLocked_Tooltip(), 1000f);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            () => this.config,
            value => this.config = value,
            configMenu => configMenu
                .AddKeybindList(
                    config => config.FreeLockKeybind,
                    (config, value) => config.FreeLockKeybind = value,
                    I18n.Config_FreeLockKeybind_Name
                )
                .AddNumberOption(
                    config => config.MoveSpeed,
                    (config, value) => config.MoveSpeed = value,
                    I18n.Config_MoveSpeed_Name,
                    I18n.Config_MoveSpeed_Tooltip
                )
                .AddNumberOption(
                    config => config.MoveThreshold,
                    (config, value) => config.MoveThreshold = value,
                    I18n.Config_MoveThreshold_Name,
                    I18n.Config_MoveThreshold_Tooltip
                )
        );
    }
}
