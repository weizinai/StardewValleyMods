using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class AutoSleepHandler : BaseHandler
{
    private bool warpingSleep;

    public AutoSleepHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        this.warpingSleep = false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (ModConfig.Instance.AutoSleep && Game1.timeOfDay >= ModConfig.Instance.AutoSleepTime)
        {
            if (Game1.activeClickableMenu is SpectatorMenu menu) menu.exitThisMenu();

            if (Game1.isWarping) return;

            if (Game1.activeClickableMenu is DialogueBox dialogueBox)
            {
                if (dialogueBox.isQuestion)
                {
                    dialogueBox.selectedResponse = 0;
                }
                dialogueBox.receiveLeftClick(0, 0);
            }

            if (!this.warpingSleep)
            {
                this.warpingSleep = true;
                var locationRequest = Game1.getLocationRequest(Game1.player.homeLocation.Value);
                locationRequest.OnWarp += () =>
                {
                    if (Game1.currentLocation is FarmHouse farmHouse)
                    {
                        Game1.player.Position = Utility.PointToVector2(farmHouse.GetPlayerBedSpot()) * 64f;
                        farmHouse.answerDialogueAction("Sleep_Yes", null);
                    }
                };
                Game1.warpFarmer(locationRequest, 5, 9, Game1.player.FacingDirection);
            }
        }
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (!ModConfig.Instance.AutoSleep || !ModConfig.Instance.SkipShippingMenu) return;

        if (e.NewMenu is ShippingMenu menu)
        {
            this.Helper.Reflection.GetMethod(menu, "okClicked").Invoke();
            Logger.Info(I18n.UI_SkipShippingMenu_Tooltip());
        }
    }
}