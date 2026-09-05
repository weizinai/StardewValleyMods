using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectateLocationHandler : BaseHandler
{
    public SpectateLocationHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (ModConfig.Instance.SpectateLocationKey.JustPressed())
        {
            var locations = Game1.locations
                .Where(this.CheckLocationAvailable)
                .Select(location => new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));

            Game1.currentLocation.ShowPagedResponses(
                I18n.UI_SpectateLocation_Title(),
                locations.ToList(),
                value => SpectatorHelper.TrySpectateLocation(value),
                false,
                true,
                10
            );
        }
    }

    private bool CheckLocationAvailable(GameLocation location)
    {
        return Game1.player.locationsVisited.Contains(location.NameOrUniqueName) &&
               (!ModConfig.Instance.OnlyShowOutdoors || location.IsOutdoors);
    }
}