using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class VersionLimitHandler : BaseHandlerWithConfig<ModConfig>
{
    private const string VersionLimitKey = "SomeMultiplayerFeature_Version";
    private const string TargetVersion = "0.10.0";

    public VersionLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.IsServer) return;

        var farmer = Game1.player;
        if (!farmer.modData.TryAdd(VersionLimitKey, TargetVersion))
            farmer.modData[VersionLimitKey] = TargetVersion;
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (Game1.IsClient) return;

        DelayedAction.functionAfterDelay(() =>
        {
            if (!this.Config.VersionLimit) return;

            var id = e.Peer.PlayerID;
            var farmer = Game1.getFarmer(id);
            if (!farmer.modData.ContainsKey(VersionLimitKey) || farmer.modData[VersionLimitKey] != TargetVersion)
            {
                this.Helper.Multiplayer.SendMessage("", "VersionLimit", new[] { "weizinai.SomeMultiplayerFeature" }, new[] { id });
                Game1.chatBox.addInfoMessage($"{farmer.Name}的<SomeMultiplayerFeature>模组不是最新版，已被踢出。");
                Game1.server.kick(id);
            }
        }, 5000);
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (Game1.IsServer) return;

        if (e is { Type: "VersionLimit", FromModID: "weizinai.SomeMultiplayerFeature" })
        {
            Log.Alert("你的<SomeMultiplayerFeature>模组不是最新版，已被踢出。");
        }
    }
}