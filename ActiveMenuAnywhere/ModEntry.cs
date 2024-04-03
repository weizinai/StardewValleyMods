using ActiveMenuAnywhere.Framework;
using Common;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ActiveMenuAnywhere;

// ReSharper disable once ClassNeverInstantiated.Global
public class ModEntry : Mod
{
    public static readonly Dictionary<MenuTabID, Texture2D> Textures = new();
    private ModConfig config = new();

    public override void Entry(IModHelper helper)
    {
        config = helper.ReadConfig<ModConfig>();
        LoadTexture();
        I18n.Init(helper.Translation);
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
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
            () => config.MenuKey,
            value => { config.MenuKey = value; },
            I18n.Config_MenuKeyName
        );

        configMenu?.AddTextOption(
            ModManifest,
            () => config.DefaultMeanTabID.ToString(),
            value =>
            {
                if (!Enum.TryParse(value, out MenuTabID tabID))
                    throw new InvalidOperationException($"Couldn't parse tab name '{value}'.");
                config.DefaultMeanTabID = tabID;
            },
            I18n.Config_DefaultMenuTabID,
            null,
            new[] { "Farm", "Town", "Mountain", "Forest", "Beach", "Desert", "GingerIsland" },
            value =>
            {
                var formatValue = value switch
                {
                    "Farm" => I18n.Tab_Farm(),
                    "Town" => I18n.Tab_Town(),
                    "Mountain" => I18n.Tab_Mountain(),
                    "Forest" => I18n.Tab_Forest(),
                    "Beach" => I18n.Tab_Beach(),
                    "Desert" => I18n.Tab_Desert(),
                    "GingerIsland" => I18n.Tab_GingerIsland(),
                    _ => ""
                };
                return formatValue;
            }
        );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else if (Context.IsPlayerFree)
                Game1.activeClickableMenu = new AMAMenu(config.DefaultMeanTabID, Helper);
        }
    }

    private void LoadTexture()
    {
        Textures.Add(MenuTabID.Farm, Helper.ModContent.Load<Texture2D>("assets/Farm.png"));
        Textures.Add(MenuTabID.Town, Helper.ModContent.Load<Texture2D>("assets/Town.png"));
        Textures.Add(MenuTabID.Mountain, Helper.ModContent.Load<Texture2D>("assets/Mountain.png"));
        Textures.Add(MenuTabID.Forest, Helper.ModContent.Load<Texture2D>("assets/Forest.png"));
        Textures.Add(MenuTabID.Beach, Helper.ModContent.Load<Texture2D>("assets/Beach.png"));
        Textures.Add(MenuTabID.Desert, Helper.ModContent.Load<Texture2D>("assets/Desert"));
        Textures.Add(MenuTabID.GingerIsland, Helper.ModContent.Load<Texture2D>("assets/GingerIsland.png"));
        Textures.Add(MenuTabID.RSV, Helper.ModContent.Load<Texture2D>("assets/RSV.png"));
    }
}