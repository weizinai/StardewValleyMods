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
        if (this.Config.SpectatePlayerKey.JustPressed() && Context.IsPlayerFree)
        {
            if (Context.HasRemotePlayers)
            {
                var players = new List<KeyValuePair<string, string>>();
                foreach (var (_, farmer) in Game1.otherFarmers)
                    players.Add(new KeyValuePair<string, string>(farmer.Name, farmer.displayName));
                Game1.currentLocation.ShowPagedResponses("", players, value => SpectatorHelper.TrySpectateFarmer(value), 
                    false, true, 10);
            }
            else
            {
                var message = new HUDMessage(I18n.UI_SpectatePlayer_Offline())
                {
                    noIcon = true
                };
                Game1.addHUDMessage(message);
            }
        }
    }
}