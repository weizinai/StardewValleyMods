using BetterCabin.Framework;
using BetterCabin.Patches;
using Common.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;

namespace BetterCabin;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private KeybindList keybind = new(SButton.O);

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Player.Warped += OnWarped;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new BuildingPatcher(config));
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (config.VisitCabinInfo && e.NewLocation is Cabin cabin)
        {
            var message = new HUDMessage(!cabin.owner.isUnclaimedFarmhand ? I18n.UI_VisitCabin_HasOwner(cabin.owner.displayName) : I18n.UI_VisitCabin_NoOwner())
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