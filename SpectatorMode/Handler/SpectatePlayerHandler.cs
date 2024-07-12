using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectatePlayerHandler : BaseHandlerWithConfig<ModConfig>
{
    public SpectatePlayerHandler(IModHelper helper) : base(helper, ModEntry.Config) { }

    public override void Apply()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.Config.SpectatePlayerKey.JustPressed())
        {
            var players = Game1.otherFarmers.Where(x => x.Key != Game1.player.UniqueMultiplayerID).Select(x => new KeyValuePair<string, string>(x.Value.Name, x.Value.displayName));

            Game1.currentLocation.ShowPagedResponses(I18n.UI_SpectatePlayer_Title(), players.ToList(),
                value => SpectatorHelper.TrySpectateFarmer(value), false, true, 10);
        }
    }
}