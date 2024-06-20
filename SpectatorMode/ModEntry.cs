using Common;
using SpectatorMode.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;

namespace SpectatorMode;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    private int cooldown;
    private bool isRotatingPlayers;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(Monitor);
        I18n.Init(helper.Translation);
        config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        cooldown++;

        if (isRotatingPlayers && cooldown >= config.RotationInterval)
        {
            cooldown = 0;
            var farmer = Game1.random.ChooseFrom(Game1.otherFarmers.Values.ToList());
            Game1.activeClickableMenu = new SpectatorMenu(farmer.currentLocation, farmer, true);
        }
        else if (Game1.activeClickableMenu is SpectatorMenu menu)
        {
            menu.exitThisMenu();
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (config.SpectatorModeKeybind.JustPressed())
        {
            var farmer = Game1.getOnlineFarmers().First(x => x.Name == "工具人");
            Game1.activeClickableMenu = new SpectatorMenu(farmer.currentLocation, farmer, true);
        }

        if (config.RotatePlayerKeybind.JustPressed() && Context.HasRemotePlayers)
            isRotatingPlayers = !isRotatingPlayers;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuForSpectatorMode(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }
}