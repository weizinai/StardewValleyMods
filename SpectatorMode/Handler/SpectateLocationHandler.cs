using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectateLocationHandler : BaseHandlerWithConfig<ModConfig>
{
    public SpectateLocationHandler(IModHelper helper) : base(helper, ModEntry.Config) { }

    public override void Apply()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.Config.SpectateLocationKey.JustPressed())
        {
            var locations = Game1.locations.Where(this.CheckLocationAvailable)
                .Select(location => new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));

            Game1.currentLocation.ShowPagedResponses(I18n.UI_SpectateLocation_Title(), locations.ToList(),
                value => SpectatorHelper.TrySpectateLocation(value), false, true, 10);
        }
    }

    private bool CheckLocationAvailable(GameLocation location)
    {
        return Game1.player.locationsVisited.Contains(location.NameOrUniqueName) &&
               (!this.Config.OnlyShowOutdoors || location.IsOutdoors);
    }
}