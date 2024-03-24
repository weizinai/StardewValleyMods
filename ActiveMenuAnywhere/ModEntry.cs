using ActiveMenuAnywhere.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere;

public class ModEntry : Mod
{
    private ModConfig config;

    public override void Entry(IModHelper helper)
    {
        config = helper.ReadConfig<ModConfig>();
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else
                Game1.activeClickableMenu = new AMAMenu(config.DefaultMeanTabLabel);
        }
    }
}