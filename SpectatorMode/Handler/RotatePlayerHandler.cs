using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class RotatePlayerHandler : BaseHandler
{
    private int cooldown;
    private bool isRotatingPlayers;

    public RotatePlayerHandler(IModHelper helper) : base(helper) { }

    public override void Init()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // 轮播玩家
        if (this.isRotatingPlayers)
        {
            if (this.cooldown >= this.Config.RotationInterval)
            {
                this.cooldown = 0;
                var farmer = Game1.random.ChooseFrom(Game1.otherFarmers.Values.ToList());
                Game1.activeClickableMenu = new SpectatorMenu(farmer.currentLocation, farmer, true);
            }
            else
            {
                this.cooldown++;
                if (Game1.activeClickableMenu is not SpectatorMenu)
                {
                    this.isRotatingPlayers = false;
                    Log.NoIconHUDMessage(I18n.UI_RotatePlayer_End());
                }
            }
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.Config.RotatePlayerKey.JustPressed())
        {
            if (Context.HasRemotePlayers)
            {
                if (this.isRotatingPlayers)
                    Game1.activeClickableMenu.exitThisMenu();
                else
                    this.cooldown = this.Config.RotationInterval;

                this.isRotatingPlayers = !this.isRotatingPlayers;
                Log.NoIconHUDMessage(this.isRotatingPlayers ? I18n.UI_RotatePlayer_Begin() : I18n.UI_RotatePlayer_End());
            }
            else
            {
                Log.ErrorHUDMessage(I18n.UI_SpectatePlayer_Offline());
            }
        }
    }
}