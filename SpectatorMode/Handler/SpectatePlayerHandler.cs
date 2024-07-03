using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectatePlayerHandler : BaseHandler
{
    public SpectatePlayerHandler(IModHelper helper) : base(helper) { }

    public override void Init()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.Config.SpectatePlayerKey.JustPressed())
        {
            var players = Game1.otherFarmers.Select(x => new KeyValuePair<string, string>(x.Value.Name, x.Value.displayName));

            Game1.currentLocation.ShowPagedResponses(I18n.UI_SpectatePlayer_Title(), players.ToList(),
                value => SpectatorHelper.TrySpectateFarmer(value), false, true, 10);
        }
    }
}