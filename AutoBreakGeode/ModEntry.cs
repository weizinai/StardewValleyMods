using AutoBreakGeode.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace AutoBreakGeode;

public class ModEntry : Mod
{
    private bool _autoBreakGeode;
    private ModConfig _config;

    public override void Entry(IModHelper helper)
    {
        _config = helper.ReadConfig<ModConfig>();
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (_config.AutoBreakGeodeKey.JustPressed())
            _autoBreakGeode = _autoBreakGeode == false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is GeodeMenu geodeMenu && _autoBreakGeode)
        {
            if (Utility.IsGeode(geodeMenu.heldItem))
            {
                var x = geodeMenu.geodeSpot.bounds.Center.X;
                var y = geodeMenu.geodeSpot.bounds.Center.Y;
                geodeMenu.receiveLeftClick(x, y);
                if (Game1.player.freeSpotsInInventory() == 1)
                    _autoBreakGeode = false;
            }
            else
            {
                _autoBreakGeode = false;
            }
        }
        else
        {
            _autoBreakGeode = false;
        }
    }
}