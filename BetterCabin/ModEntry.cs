using BetterCabin.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace BetterCabin;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Player.Warped += OnWarped;
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (config.VisitCabinInfo && e.NewLocation is Cabin cabin)
        {
            var message = new HUDMessage(I18n.UI_AccessCabin(cabin.owner.displayName))
            {
                noIcon = true,
                timeLeft = 500
            };
            Game1.addHUDMessage(message);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForBetterCabin(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }
}