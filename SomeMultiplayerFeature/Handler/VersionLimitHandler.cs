using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class VersionLimitHandler : BaseHandlerWithConfig<ModConfig>
{
    private const string VersionLimitKey = ModEntry.ModDataPrefix + "VersionLimit";
    private static string TargetVersion => "0.19.0" + " " + Game1.dayOfMonth;

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
            var id = e.Peer.PlayerID;
            var farmer = Game1.getFarmer(id);
            DelayedAction.functionAfterDelay(() =>
            {
                if (!farmer.modData.ContainsKey(VersionLimitKey) || farmer.modData[VersionLimitKey] != TargetVersion)
                {
                    var message = this.GetKickMessage(farmer);
                    Game1.chatBox.addInfoMessage(message);
                    Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, message, id);
                    Game1.server?.kick(id);
                }
            }, 10000);
        }
    }

    private string GetKickMessage(Farmer farmer)
    {
        return !farmer.modData.ContainsKey(VersionLimitKey)
            ? $"{farmer.Name}未安装<SomeMultiplayerFeature>模组，将被踢出。"
            : $"{farmer.Name}的<SomeMultiplayerFeature>模组为<{farmer.modData[VersionLimitKey]}>版本，要求的版本为<{TargetVersion}>，不满足要求，将被踢出。";
    }
}