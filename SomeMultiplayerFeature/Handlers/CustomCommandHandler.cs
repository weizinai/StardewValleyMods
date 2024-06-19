using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class CustomCommandHandler : BaseHandler
{
    private const string BannedPlayerPath = "assets/BannedPlayer.json";
    private readonly Dictionary<string, string>? bannedPlayers;

    public CustomCommandHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        bannedPlayers = helper.Data.ReadJsonFile<Dictionary<string, string>>(BannedPlayerPath);
        if (bannedPlayers is null)
            Log.Error($"无法找到Json文件: {BannedPlayerPath}，如果你不是主机玩家，则可以忽略该条消息。");
    }

    public override void Init()
    {
        Helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
        Helper.ConsoleCommands.Add("ban", "", Ban);
        Helper.ConsoleCommands.Add("unban", "", Unban);
        Helper.ConsoleCommands.Add("ping", "", Ping);
        Helper.ConsoleCommands.Add("players", "", Players);
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
        }
    }

    private void Ban(string command, string[] args)
    {
        if (!Context.IsMainPlayer) return;

        var target = Game1.getOnlineFarmers().Where(x => x.Name == args[0]);
        foreach (var farmer in target)
        {
            var id = farmer.UniqueMultiplayerID;
            var name = farmer.Name;

            if (bannedPlayers!.ContainsKey(id.ToString()))
            {
                Log.Info($"{name}已经在黑名单中。");
            }
            else
            {
                bannedPlayers.Add(id.ToString(), name);
                Game1.server.kick(id);
                Log.Info($"{name}被加入黑名单。");
            }
        }
        Helper.Data.WriteJsonFile(BannedPlayerPath, bannedPlayers);
    }

    private void Unban(string command, string[] args)
    {
        if (!Context.IsMainPlayer) return;
        
        var target = bannedPlayers!.Where(x => x.Value == args[0]).ToList();
        
        if (!target.Any()) Log.Info($"{args[0]}不在黑名单中。");
        foreach (var (id, name) in target)
        {
            bannedPlayers!.Remove(id);
            Log.Info($"{name}被移出黑名单。");
        }
        Helper.Data.WriteJsonFile(BannedPlayerPath, bannedPlayers);
    }

    private void Ping(string command, string[] args)
    {
        if (!Context.IsMainPlayer) return;

        foreach (var (id, farmer) in Game1.otherFarmers)
            Log.Info($"Ping({farmer.Name})\t\t{(int)Game1.server.getPingToClient(id)}ms ");
    }

    private void Players(string command, string[] args)
    {
        if (!Context.IsMainPlayer) return;

        foreach (var (_, farmer) in Game1.otherFarmers)
            Log.Info($"{farmer.Name}\t\t{farmer.currentLocation.Name}");
    }
}