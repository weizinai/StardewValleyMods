using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectateLocationHandler : BaseHandler
{
    public override void Init()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;
        
        if (this.Config.SpectateLocationKey.JustPressed())
        {
            var locations = Game1.locations
                .Where(location => !this.Config.OnlyShowOutdoors || location.IsOutdoors)
                .Select(location => new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));
            
            Game1.currentLocation.ShowPagedResponses(I18n.UI_SpectateLocation_Title(), locations.ToList(), 
                value => SpectatorHelper.TrySpectateLocation(value), false, true, 10);
        }
    }
}