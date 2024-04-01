using AutoBreakGeode.Framework;
using Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace AutoBreakGeode;

public class ModEntry : Mod
{
    private bool autoBreakGeode;
    private ModConfig config = new();

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        config = helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        
        configMenu?.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );
        
        configMenu?.AddKeybindList(
            ModManifest,
            () => config.AutoBreakGeodeKey,
            value => { config.AutoBreakGeodeKey = value; },
            I18n.Config_AutoBreakGeodeKey
        );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.AutoBreakGeodeKey.JustPressed())
            autoBreakGeode = autoBreakGeode == false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is GeodeMenu geodeMenu && autoBreakGeode)
        {
            if (Utility.IsGeode(geodeMenu.heldItem))
            {
                var x = geodeMenu.geodeSpot.bounds.Center.X;
                var y = geodeMenu.geodeSpot.bounds.Center.Y;
                geodeMenu.receiveLeftClick(x, y);
                if (Game1.player.freeSpotsInInventory() == 1)
                    autoBreakGeode = false;
            }
            else
            {
                autoBreakGeode = false;
            }
        }
        else
        {
            autoBreakGeode = false;
        }
    }
}