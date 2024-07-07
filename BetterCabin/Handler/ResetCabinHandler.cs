using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.Common.Handler;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class ResetCabinHandler : BaseHandlerWithConfig<ModConfig>
{
    public ResetCabinHandler(IModHelper helper, ModConfig config) : base(helper, config) { }

    public override void Init()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Context.IsPlayerFree) return;

        if (this.Config.ResetCabinPlayerKeybind.JustPressed())
        {
            var location = Game1.player.currentLocation;
            if (location is Cabin cabin)
            {
                if (Game1.player.team.playerIsOnline(cabin.owner.UniqueMultiplayerID))
                {
                    Game1.addHUDMessage(new HUDMessage(I18n.UI_ResetCabin_Online()) { noIcon = true });
                    return;
                }

                if (!cabin.owner.isUnclaimedFarmhand)
                    this.ResetCabin(cabin);
                else
                    Game1.addHUDMessage(new HUDMessage(I18n.UI_ResetCabin_NoOwner()) { noIcon = true });
            }
            else
            {
                var farmhands = Game1.getAllFarmhands()
                    .Where(farmer => !farmer.isUnclaimedFarmhand)
                    .Select(farmer => new KeyValuePair<string, string>(farmer.UniqueMultiplayerID.ToString(), farmer.displayName));
                location.ShowPagedResponses(I18n.UI_ResetCabin_ChooseFarmhand(), farmhands.ToList(), value =>
                {
                    var id = long.Parse(value);
                    if (Game1.player.team.playerIsOnline(id)) Game1.server.kick(id);
                    var farmer = Game1.getFarmer(id);
                    this.ResetCabin((Utility.getHomeOfFarmer(farmer) as Cabin)!);
                });
            }
        }
    }

    private void ResetCabin(Cabin cabin)
    {
        Game1.addHUDMessage(new HUDMessage(I18n.UI_ResetCabin_Success(cabin.owner.displayName)) { noIcon = true });
        cabin.DeleteFarmhand();
        cabin.CreateFarmhand();
    }
}