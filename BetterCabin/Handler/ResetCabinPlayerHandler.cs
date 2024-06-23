using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class ResetCabinPlayerHandler : BaseHandler
{
    public ResetCabinPlayerHandler(ModConfig config, IModHelper helper) : base(config, helper)
    {
    }

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

        if (this.Config.DeleteFarmhandKeybind.JustPressed())
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