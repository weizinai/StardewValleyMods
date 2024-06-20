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

    // 轮播玩家
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
        // 轮播玩家
        if (isRotatingPlayers)
        {
            if (cooldown >= config.RotationInterval)
            {
                cooldown = 0;
                var farmer = Game1.random.ChooseFrom(Game1.otherFarmers.Values.ToList());
                Game1.activeClickableMenu = new SpectatorMenu(farmer.currentLocation, farmer, true);
            }
            else
            {
                cooldown++;
                if (Game1.activeClickableMenu is not SpectatorMenu) isRotatingPlayers = false;
            }
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        // 轮播玩家
        if (config.RotatePlayerKeybind.JustPressed() && Context.HasRemotePlayers)
        {
            if (isRotatingPlayers)
                Game1.activeClickableMenu.exitThisMenu();
            else
                cooldown = config.RotationInterval;
            
            isRotatingPlayers = !isRotatingPlayers;
            var message = new HUDMessage(isRotatingPlayers ? I18n.UI_StartRotatePlayer() : I18n.UI_StopRotatePlayer())
            {
                noIcon = true
            };
            Game1.addHUDMessage(message);
        }
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