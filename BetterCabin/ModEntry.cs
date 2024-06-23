using System.Text;
using weizinai.StardewValleyMod.Common.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
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
        this.config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        helper.Events.Player.Warped += this.OnWarped;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new BuildingPatcher(this.config));
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        this.SetCabinSkin();
        this.DeleteFarmhand();
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        this.VisitCabinInfo(e);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForBetterCabin(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }

    // 拜访小屋信息
    private void VisitCabinInfo(WarpedEventArgs e)
    {
        if (this.config.VisitCabinInfo && e.NewLocation is Cabin cabin)
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

            Game1.addHUDMessage(new HUDMessage(messageContent.ToString()) { noIcon = true });
        }
    }

    // 设置小屋皮肤
    public void SetCabinSkin()
    {
        if (Context.IsMainPlayer || !Context.IsPlayerFree) return;

        if (this.config.SetCabinSkin && this.config.SetCabinSkinKeybind.JustPressed())
        {
            Utility.ForEachBuilding(building =>
            {
                if (building.GetIndoors() is Cabin cabin && cabin.owner.Equals(Game1.player))
                {
                    Game1.activeClickableMenu = new BuildingSkinMenu(building, true);
                    return false;
                }

                return true;
            });
        }
    }

    // 删除小屋主人
    private void DeleteFarmhand()
    {
        if (!Context.IsMainPlayer || !Context.IsPlayerFree) return;

        if (this.config.DeleteFarmhand && this.config.DeleteFarmhandKeybind.JustPressed())
        {
            var location = Game1.player.currentLocation;
            if (location is Cabin cabin)
            {
                if (!cabin.owner.isUnclaimedFarmhand)
                    this.ResetCabinPlayer(cabin);
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
                    this.ResetCabinPlayer((Utility.getHomeOfFarmer(farmer) as Cabin)!);
                });
            }
        }
    }

    private void ResetCabinPlayer(Cabin cabin)
    {
        Game1.addHUDMessage(new HUDMessage(I18n.UI_DeleteFarmhand_Success(cabin.owner.displayName)) { noIcon = true });
        cabin.DeleteFarmhand();
        cabin.CreateFarmhand();
    }
}