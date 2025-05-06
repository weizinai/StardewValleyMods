using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class SpectatePlayerHandler : BaseHandler
{
    private bool autoSpectatePlayer;

    public SpectatePlayerHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (ModConfig.Instance.SpectatePlayerKey.JustPressed())
        {
            var players = Game1.getOnlineFarmers()
                .Where(x => x.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                .Select(x => new KeyValuePair<string, string>(x.Name, x.displayName));

            Game1.currentLocation.ShowPagedResponses(
                I18n.UI_SpectatePlayer_Title(),
                players.ToList(),
                value => SpectatorHelper.TrySpectateFarmer(value, out _),
                false,
                true,
                10
            );
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.getOnlineFarmers().Count == 1) return;

        if (!this.autoSpectatePlayer && ModConfig.Instance.AutoSpectatePlayer && Game1.timeOfDay == ModConfig.Instance.AutoSpectatePlayerTime)
        {
            this.EnableAutoSpectatePlayer();
        }

        if (this.autoSpectatePlayer && Game1.activeClickableMenu is not SpectatorMenu)
        {
            var randomPlayer = Game1.random.ChooseFrom(Game1.getOnlineFarmers()
                .Where(x => x.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                .ToList()
            );

            if (randomPlayer == null)
            {
                this.autoSpectatePlayer = false;
                Logger.NoIconHUDMessage(I18n.UI_SpectatePlayer_Offline());
                return;
            }

            if (SpectatorHelper.TrySpectateFarmer(randomPlayer.displayName, out var menu))
            {
                menu.RandomSpectate = true;
                menu.exitFunction += () =>
                {
                    this.autoSpectatePlayer = false;
                    Logger.NoIconHUDMessage(I18n.UI_AutoSpectatePlayer_Disable());
                };
            }
        }
    }

    private void EnableAutoSpectatePlayer()
    {
        this.autoSpectatePlayer = true;
        Logger.NoIconHUDMessage(I18n.UI_AutoSpectatePlayer_Enable());
    }
}