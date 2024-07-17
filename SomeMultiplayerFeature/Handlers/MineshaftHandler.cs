using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class MineshaftHandler : BaseHandler
{
    public MineshaftHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Player.Warped += this.OnWarped;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    public override void Clear()
    {
        this.Helper.Events.Player.Warped -= this.OnWarped;
        this.Helper.Events.Multiplayer.ModMessageReceived -= this.OnModMessageReceived;
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (Game1.IsServer) return;

        if (e is { OldLocation: MineShaft, NewLocation: not MineShaft })
        {
            this.Helper.Multiplayer.SendMessage("", "RefreshMineshaft", new[] { "weizinai.SomeMultiplayerFeature" },
                new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            Log.NoIconHUDMessage("矿井已刷新", 500f);
        }
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (Game1.IsClient) return;

        if (e.Type == "RefreshMineshaft") RefreshMineshaft();
    }

    public static void RefreshMineshaft()
    {
        MineShaft.activeMines.RemoveAll(mine => mine.mineLevel <= 120 && !mine.farmers.Any());
    }
}