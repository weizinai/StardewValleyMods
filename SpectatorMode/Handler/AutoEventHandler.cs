using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class AutoEventHandler : BaseHandler
{
    public AutoEventHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!ModConfig.Instance.AutoSkipEvent) return;

        if (Game1.CurrentEvent != null)
        {
            if (!Game1.CurrentEvent.skipped && Game1.CurrentEvent.skippable)
            {
                Game1.CurrentEvent.skippable = true;
                Game1.CurrentEvent.skipEvent();
                Game1.freezeControls = false;
            }
        }
    }
}