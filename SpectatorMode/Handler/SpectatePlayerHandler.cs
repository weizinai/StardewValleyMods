using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectatePlayerHandler : BaseHandler
{
    public SpectatePlayerHandler(IModHelper helper, ModConfig config) : base(helper, config) { }

    public override void Init()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.Config.SpectatePlayerKey.JustPressed())
        {
            var players = new List<KeyValuePair<string, string>>();
            foreach (var (_, farmer) in Game1.otherFarmers) players.Add(new KeyValuePair<string, string>(farmer.Name, farmer.displayName));
            Game1.currentLocation.ShowPagedResponses(I18n.UI_SpectatePlayer_Title(), players, 
                value => SpectatorHelper.TrySpectateFarmer(value), false, true, 10);
        }
    }
}