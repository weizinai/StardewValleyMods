using Common;
using FreeLock.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FreeLock;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        Log.Init(Monitor);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || !Game1.viewportFreeze) return;
        
        var mouseX = Game1.getOldMouseX(false);
        var mouseY = Game1.getOldMouseY(false);
        if (mouseX < 64)
        {
            Game1.panScreen(-32, 0);
        }
        else if (mouseX - Game1.viewport.Width >= -128)
        {
            Game1.panScreen(32, 0);
        }
        if (mouseY < 64)
        {
            Game1.panScreen(0, -32);
        }
        else if (mouseY - Game1.viewport.Height >= -64)
        {
            Game1.panScreen(0, 32);
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsWorldReady) return;

        if (config.FreeLockKeybind.JustPressed())
        {
            if (Game1.viewportFreeze)
            {
                Game1.viewportFreeze = false;
                Game1.addHUDMessage(new HUDMessage(I18n.UI_ViewportLocked()));
            }
            else
            {
                Game1.viewportFreeze = true;
                Game1.addHUDMessage(new HUDMessage(I18n.UI_ViewportUnlocked()));
            }
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuForFreeLock(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }
}