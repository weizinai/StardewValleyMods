using ActiveMenuAnywhere.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ActiveMenuAnywhere;

public class ModEntry : Mod
{
    private readonly Dictionary<MenuTabID, Texture2D> textures = new();
    private ModConfig config = new();

    public override void Entry(IModHelper helper)
    {
        config = helper.ReadConfig<ModConfig>();
        LoadTexture();
        I18n.Init(helper.Translation);
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else if (Context.IsPlayerFree)
                Game1.activeClickableMenu = new AMAMenu(config.DefaultMeanTabID, Helper, textures);
        }
    }

    private void LoadTexture()
    {
        textures.Add(MenuTabID.Farm, Helper.ModContent.Load<Texture2D>("assets/Farm.png"));
        textures.Add(MenuTabID.Town1, Helper.ModContent.Load<Texture2D>("assets/Town1.png"));
        textures.Add(MenuTabID.Town2, Helper.ModContent.Load<Texture2D>("assets/Town2.png"));
        textures.Add(MenuTabID.Mountain, Helper.ModContent.Load<Texture2D>("assets/Mountain.png"));
        textures.Add(MenuTabID.Forest, Helper.ModContent.Load<Texture2D>("assets/Forest.png"));
        textures.Add(MenuTabID.Beach, Helper.ModContent.Load<Texture2D>("assets/Beach.png"));
        textures.Add(MenuTabID.Desert, Helper.ModContent.Load<Texture2D>("assets/Desert"));
        textures.Add(MenuTabID.GingerIsland, Helper.ModContent.Load<Texture2D>("assets/GingerIsland.png"));
    }
}