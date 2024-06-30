using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectateLocationHandler : BaseHandler
{
    public SpectateLocationHandler(IModHelper helper, ModConfig config) : base(helper, config) { }

    public override void Init()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.Config.SpectateLocationKey.JustPressed() && Context.IsPlayerFree)
        {
            var locations = Game1.locations.Where(location => location.IsOutdoors)
                .Select(location => new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName)).ToList();
            Game1.currentLocation.ShowPagedResponses("", locations, value => SpectatorHelper.TrySpectateLocation(value), 
                false, true, 10);
        }
    }
}