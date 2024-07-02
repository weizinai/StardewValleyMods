using StardewModdingAPI;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class CommandHandler : BaseHandler
{
    public CommandHandler(IModHelper helper) : base(helper) { }

    public override void Init()
    {
        this.Helper.ConsoleCommands.Add("spectate_location", "", this.SpectateLocation);
        this.Helper.ConsoleCommands.Add("spectate_player", "", this.SpectateFarmer);
    }

    // 旁观地点
    private void SpectateLocation(string command, string[] args)
    {
        var locationName = args[0];

        Log.Info(SpectatorHelper.TrySpectateLocation(locationName)
            ? I18n.UI_SpectateLocation_Success(locationName)
            : I18n.UI_SpectateLocation_Fail(locationName));
    }

    // 旁观玩家
    private void SpectateFarmer(string command, string[] args)
    {
        var playerName = args[0];

        Log.Info(SpectatorHelper.TrySpectateFarmer(playerName)
            ? I18n.UI_SpectatePlayer_Success(playerName)
            : I18n.UI_SpectatePlayer_Fail(playerName));
    }
}