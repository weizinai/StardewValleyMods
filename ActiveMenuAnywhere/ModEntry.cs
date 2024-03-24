using ActiveMenuAnywhere.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere;

public class ModEntry : Mod
{
    private ModConfig config = new();
    private Dictionary<MenuTabID, Texture2D> textures = new();

    public override void Entry(IModHelper helper)
    {
        config = helper.ReadConfig<ModConfig>();
        textures.Add(MenuTabID.Farm, helper.ModContent.Load<Texture2D>("assets/Farm.png"));
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else
                Game1.activeClickableMenu = new AMAMenu(config.DefaultMeanTabID, Helper, textures);
        }
    }
}