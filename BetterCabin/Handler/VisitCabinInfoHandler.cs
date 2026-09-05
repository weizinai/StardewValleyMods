using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class VisitCabinInfoHandler : BaseHandler
{
    public VisitCabinInfoHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Player.Warped += this.OnWarped;
    }

    public override void Clear()
    {
        this.Helper.Events.Player.Warped -= this.OnWarped;
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation is Cabin cabin)
        {
            var owner = cabin.owner;
            var messageContent = new StringBuilder();

            if (owner.isUnclaimedFarmhand)
            {
                messageContent.Append(I18n.UI_VisitCabin_NoOwner());
            }
            else
            {
                var isOnline = owner.IsOnline();
                messageContent.Append(I18n.UI_VisitCabin_HasOwner(owner.Name));
                messageContent.Append('\n');
                messageContent.Append(isOnline ? I18n.UI_VisitCabin_Online() : I18n.UI_VisitCabin_Offline());
                messageContent.Append('\n');
                messageContent.Append(I18n.UI_VisitCabin_TotalOnlineTime(Utility.getHoursMinutesStringFromMilliseconds(owner.millisecondsPlayed)));
                if (!isOnline)
                {
                    messageContent.Append('\n');
                    messageContent.Append(I18n.UI_VisitCabin_LastOnlineTime(Utility.getDateString(-((int)Game1.stats.DaysPlayed - owner.disconnectDay.Value))));
                }
            }

            Logger.NoIconHUDMessage(messageContent.ToString());
        }
    }
}