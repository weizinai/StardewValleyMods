using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class AutoFestivalHandler : BaseHandler
{
    private bool warpingFestival;

    public AutoFestivalHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        this.warpingFestival = false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (ModConfig.Instance.AutoParticipateFestival && Game1.whereIsTodaysFest is not null)
        {
            if (Game1.activeClickableMenu is SpectatorMenu menu) menu.exitThisMenu();

            if (Game1.isWarping) return;

            if (!this.warpingFestival && Game1.timeOfDay >= Utility.getStartTimeOfFestival())
            {
                this.warpingFestival = true;
                var locationRequest = Game1.getLocationRequest(Game1.whereIsTodaysFest);
                var tileX = -1;
                var tileY = -1;
                Utility.getDefaultWarpLocation(Game1.whereIsTodaysFest, ref tileX, ref tileY);
                Game1.warpFarmer(locationRequest, tileX, tileY, 2);
            }
        }
    }
}