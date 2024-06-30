using StardewModdingAPI;
using weizinai.StardewValleyMod.SpectatorMode.Framework;

namespace weizinai.StardewValleyMod.SpectatorMode.Handler;

internal class CommandHandler : BaseHandler
{
    public CommandHandler(IModHelper helper, ModConfig config) : base(helper, config) { }

    public override void Init()
    {
        this.Helper.ConsoleCommands.Add("spectate_location", "", this.SpectateLocation);
        this.Helper.ConsoleCommands.Add("spectate_player", "", this.SpectateFarmer);
    }

    // 旁观地点
    private void SpectateLocation(string command, string[] args)
    {
        SpectatorHelper.SpectateLocation(args[0]);
    }

    // 旁观玩家
    private void SpectateFarmer(string command, string[] args)
    {
        SpectatorHelper.SpectateFarmer(args[0]);
    }
}