using weizinai.StardewValleyMod.Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode;

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
        // 自定义命令
        helper.ConsoleCommands.Add("spectate_location", "", SpectateLocation);
        helper.ConsoleCommands.Add("spectate_player", "", SpectateFarmer);
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
                Game1.activeClickableMenu = new SpectatorMenu(config, farmer.currentLocation, farmer, true);
            }
            else
            {
                cooldown++;
                if (Game1.activeClickableMenu is not SpectatorMenu)
                {
                    isRotatingPlayers = false;
                    var message = new HUDMessage(I18n.UI_StopRotatePlayer())
                    {
                        noIcon = true
                    };
                    Game1.addHUDMessage(message);
                }
            }
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        // 旁观地点
        if (config.SpectateLocationKeybind.JustPressed() && Context.IsPlayerFree)
        {
            var locations = Game1.locations.Where(location => location.IsOutdoors)
                .Select(location => new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName)).ToList();
            Game1.currentLocation.ShowPagedResponses("", locations, value => SpectateLocation("spectate_location", new[] { value }),
                false, true, 10);
        }

        // 旁观玩家
        if (config.SpectatePlayerKeybind.JustPressed() && Context.IsPlayerFree)
        {
            if (Context.HasRemotePlayers)
            {
                var players = new List<KeyValuePair<string, string>>();
                foreach (var (_, farmer) in Game1.otherFarmers)
                    players.Add(new KeyValuePair<string, string>(farmer.Name, farmer.displayName));
                Game1.currentLocation.ShowPagedResponses("", players, value => SpectateFarmer("spectate_player", new[] { value }),
                    false, true, 10);
            }
            else
            {
                var message = new HUDMessage(I18n.UI_NoPlayerOnline())
                {
                    noIcon = true
                };
                Game1.addHUDMessage(message);
            }
        }

        // 轮播玩家
        if (config.RotatePlayerKeybind.JustPressed())
        {
            if (Context.HasRemotePlayers)
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
            else
            {
                var message = new HUDMessage(I18n.UI_NoPlayerOnline())
                {
                    noIcon = false
                };
                Game1.addHUDMessage(message);
            }
        }
    }

    // 旁观地点
    private void SpectateLocation(string command, string[] args)
    {
        var location = Game1.getLocationFromName(args[0]);

        if (location is null)
        {
            Log.Info(I18n.UI_SpectateLocation_Fail(args[0]));
            return;
        }

        Game1.activeClickableMenu = new SpectatorMenu(config, location);
        Log.Info(I18n.UI_SpectateLocation_Success(location.DisplayName));
    }

    // 旁观玩家
    private void SpectateFarmer(string command, string[] args)
    {
        if (!Context.HasRemotePlayers)
        {
            Log.Info(I18n.UI_NoPlayerOnline());
            return;
        }

        var farmer = Game1.otherFarmers.FirstOrDefault(x => x.Value.Name == args[0]).Value;

        if (farmer is null)
        {
            Log.Info(I18n.UI_SpectatePlayer_Fail(args[0]));
            return;
        }

        Game1.activeClickableMenu = new SpectatorMenu(config, farmer.currentLocation, farmer, true);
        Log.Info(I18n.UI_SpectatePlayer_Success(farmer.Name));
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