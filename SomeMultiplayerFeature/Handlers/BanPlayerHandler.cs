using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class BanPlayerHandler : BaseHandler
{
    private const string BannedPlayerPath = "assets/BannedPlayer.json";
    private readonly Dictionary<string, string>? bannedPlayers;
    private readonly Dictionary<long, string> onlineFarmers = new();

    public BanPlayerHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        bannedPlayers = helper.Data.ReadJsonFile<Dictionary<string, string>>(BannedPlayerPath);
        if (bannedPlayers is null)
            Log.Error($"无法找到Json文件: {BannedPlayerPath}，如果你不是主机玩家，则可以忽略该条消息。");
    }

    public override void Init()
    {
        Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        Helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
        Helper.Events.Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Helper.ConsoleCommands.Add("ban", "", BanPlayer);
        Helper.ConsoleCommands.Add("unban", "", UnbanPlayer);
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        onlineFarmers.Clear();
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (!Context.IsMainPlayer || bannedPlayers is null) return;

        var id = e.Peer.PlayerID;
        var name = Game1.getFarmer(id).Name;

        if (bannedPlayers.ContainsKey(id.ToString()))
        {
            Game1.server.kick(id);
            Log.Alert($"{name}在黑名单中，已被踢出。");
            return;
        }
        
        onlineFarmers.TryAdd(id, name);
    }

    private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        onlineFarmers.Remove(e.Peer.PlayerID);
    }

    private void BanPlayer(string command, string[] args)
    {
        if (!Context.IsMainPlayer) return;

        var target = onlineFarmers.Where(x => x.Value == args[0]);
        foreach (var (id, name) in target)
        {
            if (bannedPlayers!.TryAdd(id.ToString(), name))
            {
                Game1.server.kick(id);
                Log.Info($"{name}被加入黑名单。");
            }
            else
            {
                Log.Info($"{name}已经在黑名单中。");
            }
        }
        Helper.Data.WriteJsonFile(BannedPlayerPath, bannedPlayers);
    }

    private void UnbanPlayer(string command, string[] args)
    {
        if (!Context.IsMainPlayer) return;
        
        var target = bannedPlayers!.Where(x => x.Value == args[0]);
        foreach (var (id, name) in target)
        {
            Log.Info(bannedPlayers!.Remove(id) ? $"{name}被移出黑名单。" : $"{name}不在黑名单中。");
        }
        Helper.Data.WriteJsonFile(BannedPlayerPath, bannedPlayers);
    }
}