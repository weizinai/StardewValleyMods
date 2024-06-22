using System.Text;
using weizinai.StardewValleyMod.Common.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin;

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
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
        helper.Events.Player.Warped += OnWarped;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new BuildingPatcher(config));
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        if (config.DeleteFarmhand && config.DeleteFarmhandKeybind.JustPressed())
        {
            var location = Game1.player.currentLocation;
            if (location is Cabin cabin)
            {
                if (!cabin.owner.isUnclaimedFarmhand)
                    DeleteFarmhand(cabin);
                else
                    Game1.addHUDMessage(new HUDMessage(I18n.UI_DeleteFarmhand_NoOwner()) { noIcon = true });
            }
            else
            {
                var farmhands = Game1.getAllFarmhands()
                    .Where(farmer => !farmer.isUnclaimedFarmhand)
                    .Select(farmer => new KeyValuePair<string, string>(farmer.UniqueMultiplayerID.ToString(), farmer.displayName));
                location.ShowPagedResponses(I18n.UI_DeleteFarmhand_ChooseFarmhand(), farmhands.ToList(), value =>
                {
                    var farmer = Game1.getFarmer(long.Parse(value));
                    DeleteFarmhand((Utility.getHomeOfFarmer(farmer) as Cabin)!);
                });
            }
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (config.VisitCabinInfo && e.NewLocation is Cabin cabin)
        {
            var owner = cabin.owner;
            var messageContent = new StringBuilder();

            if (owner.isUnclaimedFarmhand)
            {
                messageContent.Append(I18n.UI_VisitCabin_NoOwner());
            }
            else
            {
                var isOnline = Game1.player.team.playerIsOnline(owner.UniqueMultiplayerID);
                messageContent.Append(I18n.UI_VisitCabin_HasOwner(owner.displayName));
                messageContent.Append('\n');
                messageContent.Append(isOnline ? I18n.UI_VisitCabin_Online() : I18n.UI_VisitCabin_Offline());
                messageContent.Append('\n');
                messageContent.Append(I18n.UI_VisitCabin_TotalOnlineTime(Utility.getHoursMinutesStringFromMilliseconds(owner.millisecondsPlayed)));
                if (!isOnline)
                {
                    messageContent.Append('\n');
                    messageContent.Append(I18n.UI_VisitCabin_LastOnlineTime(Utility.getDateString((int)(Game1.stats.DaysPlayed - owner.disconnectDay.Value))));
                }
            }
            
            var message = new HUDMessage(messageContent.ToString())
            {
                noIcon = true,
                timeLeft = 1000
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

    private void DeleteFarmhand(Cabin cabin)
    {
        Game1.addHUDMessage(new HUDMessage(I18n.UI_DeleteFarmhand_Success(cabin.owner.displayName)) { noIcon = true });
        cabin.DeleteFarmhand();
        cabin.CreateFarmhand();
    }
}