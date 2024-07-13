using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.Common.Extension;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class ResetCabinHandler : BaseHandlerWithConfig<ModConfig>
{
    public ResetCabinHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Game1.IsClient || !Context.IsPlayerFree) return;

        if (this.Config.ResetCabinPlayerKeybind.JustPressed())
        {
            var location = Game1.player.currentLocation;
            if (location is Cabin cabin)
            {
                if (cabin.owner.IsOnline())
                {
                    Log.NoIconHUDMessage(I18n.UI_ResetCabin_Online());
                    return;
                }

                if (!cabin.owner.isUnclaimedFarmhand)
                    this.ResetCabin(cabin);
                else
                    Log.NoIconHUDMessage(I18n.UI_ResetCabin_NoOwner());
            }
            else
            {
                var farmhands = Game1.getAllFarmhands()
                    .Where(farmer => !farmer.isUnclaimedFarmhand)
                    .Select(farmer => new KeyValuePair<string, string>(farmer.UniqueMultiplayerID.ToString(), farmer.Name));

                location.ShowPagedResponses(I18n.UI_ResetCabin_ChooseFarmhand(), farmhands.ToList(), value =>
                {
                    var id = long.Parse(value);
                    var farmer = Game1.getFarmer(id);
                    if (farmer.IsOnline()) Game1.server.kick(id);
                    this.ResetCabin((Utility.getHomeOfFarmer(farmer) as Cabin)!);
                });
            }
        }
    }

    private void ResetCabin(Cabin cabin)
    {
        Log.NoIconHUDMessage(I18n.UI_ResetCabin_Success(cabin.owner.Name));
        cabin.DeleteFarmhand();
        cabin.CreateFarmhand();
    }
}