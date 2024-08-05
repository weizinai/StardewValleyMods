using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class VersionLimitHandler : BaseHandlerWithConfig<ModConfig>
{
    private const string VersionLimitKey = ModEntry.ModDataPrefix + "VersionLimit";
    private static string TargetVersion => "0.19.2" + "_" + Game1.dayOfMonth;

    private readonly List<PlayerToKickData> datas = new();

    public VersionLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
        this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnOneSecondUpdateTicked;
        this.Helper.Events.Multiplayer.PeerConnected -= this.OnPeerConnected;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.IsClient) Game1.player.modData[VersionLimitKey] = TargetVersion;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!this.IsVersionLimitEnable()) return;

        foreach (var data in this.datas)
        {
            data.TimeLeft--;

            if (data.TimeLeft < 0)
            {
                var farmer = Game1.getFarmerMaybeOffline(data.Id);

                if (farmer == null)
                {
                    Game1.chatBox.addInfoMessage($"无法获取Id为{data.Id}的玩家，该玩家可能已经退出。");
                    return;
                }

                if (!farmer.modData.ContainsKey(VersionLimitKey) || farmer.modData[VersionLimitKey] != TargetVersion)
                {
                    var message = this.GetKickMessage(farmer);
                    Game1.chatBox.addInfoMessage(message);
                    try
                    {
                        Game1.server.kick(farmer.UniqueMultiplayerID);
                    }
                    catch (Exception)
                    {
                        Game1.chatBox.addErrorMessage($"踢出玩家{farmer.Name}失败，该问题可能导致空用户的产生，已尝试解决。");
                        Game1.otherFarmers.Remove(data.Id);
                    }
                }
            }
        }

        this.datas.RemoveAll(data => data.TimeLeft < 0);
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (!this.IsVersionLimitEnable()) return;

        this.datas.Add(new PlayerToKickData(e.Peer.PlayerID, 10));
    }

    private string GetKickMessage(Farmer farmer)
    {
        return !farmer.modData.ContainsKey(VersionLimitKey)
            ? $"{farmer.Name}未安装<SomeMultiplayerFeature>模组，将被踢出。"
            : $"{farmer.Name}的<SomeMultiplayerFeature>模组为<{farmer.modData[VersionLimitKey]}>版本，要求的版本为<{TargetVersion}>，不满足要求，将被踢出。";
    }

    private bool IsVersionLimitEnable()
    {
        return Game1.IsServer && this.Config.VersionLimit;
    }
}