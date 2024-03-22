using ActiveMenuAnywhere.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere;

public class ModEntry : Mod
{
    private ModConfig _config;

    public override void Entry(IModHelper helper)
    {
        _config = helper.ReadConfig<ModConfig>();
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (_config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is Menu)
                Game1.exitActiveMenu();
            else
                Game1.activeClickableMenu = new Menu();
        }
    }
}