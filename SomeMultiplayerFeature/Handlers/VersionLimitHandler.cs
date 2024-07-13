using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class VersionLimitHandler : BaseHandlerWithConfig<ModConfig>
{
    private const string VersionLimitKey = "weizinai.SomeMultiplayerFeature_Version";
    private const string TargetVersion = "0.13.0";

    public VersionLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
        this.Helper.Events.Multiplayer.PeerConnected -= this.OnPeerConnected;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.IsClient) Game1.player.modData[VersionLimitKey] = TargetVersion;
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (this.Config.VersionLimit && Game1.IsServer)
        {
            DelayedAction.functionAfterDelay(() =>
            {
                var id = e.Peer.PlayerID;
                var farmer = Game1.getFarmer(id);
                if (!farmer.modData.ContainsKey(VersionLimitKey) || farmer.modData[VersionLimitKey] != TargetVersion)
                {
                    Game1.chatBox.addInfoMessage($"{farmer.Name}的<SomeMultiplayerFeature>模组不是最新版，将被踢出。");
                    Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, "你的<SomeMultiplayerFeature>模组不是最新版，将被踢出。", id);
                    Game1.server.kick(id);
                }
            }, 5000);
        }
    }
}