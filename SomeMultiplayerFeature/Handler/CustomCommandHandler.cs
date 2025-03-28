using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class CustomCommandHandler : BaseHandlerWithConfig<ModConfig>
{
    private const string BannedPlayerPath = "assets/BannedPlayer.json";
    private readonly Dictionary<string, string>? bannedPlayers;

    public CustomCommandHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        this.bannedPlayers = helper.Data.ReadJsonFile<Dictionary<string, string>>(BannedPlayerPath);
        if (this.bannedPlayers is null)
        {
            this.bannedPlayers = new Dictionary<string, string>();
            this.Helper.Data.WriteJsonFile(BannedPlayerPath, this.bannedPlayers);
        }
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.GameLaunched -= this.OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;

        this.Helper.ConsoleCommands.Add("ban", "", this.BanPlayer);
        this.Helper.ConsoleCommands.Add("unban", "", this.UnbanPlayer);
        this.Helper.ConsoleCommands.Add("ping", "", this.PingPlayer);
        this.Helper.ConsoleCommands.Add("list", "", this.ListPlayer);
        this.Helper.ConsoleCommands.Add("kick", "", this.KickPlayer);
        this.Helper.ConsoleCommands.Add("kickall", "", this.KickAllPlayer);

        this.Helper.ConsoleCommands.Add("inventory", "", this.AccessInventory);

        this.Helper.ConsoleCommands.Add("server_mode", "", this.SetServerMode);
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        // 如果当前不是联机模式或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        var id = e.Peer.PlayerID;
        var player = Game1.GetPlayer(id);
        if (player is null)
        {
            Log.Error($"无法获取玩家{id}的姓名");
            return;
        }

        if (this.bannedPlayers!.ContainsKey(id.ToString()))
        {
            Game1.server.kick(id);
            Log.Alert($"{player.Name}在黑名单中，已被踢出。");
        }
    }

    private void BanPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var target = Game1.getOnlineFarmers().Where(x => x.Name == args[0]);
        foreach (var farmer in target)
        {
            var id = farmer.UniqueMultiplayerID;
            var name = farmer.Name;

            if (this.bannedPlayers!.ContainsKey(id.ToString()))
            {
                Log.Info($"{name}已经在黑名单中。");
            }
            else
            {
                this.bannedPlayers.Add(id.ToString(), name);
                Game1.server.kick(id);
                Log.Info($"{name}被加入黑名单。");
            }
        }

        this.Helper.Data.WriteJsonFile(BannedPlayerPath, this.bannedPlayers);
    }

    private void UnbanPlayer(string command, string[] args)
    {
        // 如果当前不是联机模式或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        var target = this.bannedPlayers!.Where(x => x.Value == args[0]).ToList();

        if (!target.Any()) Log.Info($"{args[0]}不在黑名单中。");
        foreach (var (id, name) in target)
        {
            this.bannedPlayers!.Remove(id);
            Log.Info($"{name}被移出黑名单。");
        }

        this.Helper.Data.WriteJsonFile(BannedPlayerPath, this.bannedPlayers);
    }

    private void PingPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        foreach (var (id, farmer) in Game1.otherFarmers)
            Log.Info($"{Game1.Multiplayer.getUserName(farmer.UniqueMultiplayerID)} {farmer.Name}\t{(int)Game1.server.getPingToClient(id)}ms ");
    }

    private void ListPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        Log.Info("下面是在线的玩家：");
        foreach (var farmer in Game1.getOnlineFarmers())
            Log.Info($"{farmer.Name}\t{farmer.currentLocation.DisplayName}\t{Game1.Multiplayer.getUserName(farmer.UniqueMultiplayerID)}");
        Log.Info("下面是离线的玩家");
        foreach (var farmer in Game1.getOfflineFarmhands())
            Log.Info($"{farmer.Name}");
    }

    private void KickPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var target = Game1.getOnlineFarmers().FirstOrDefault(x => x.Name == args[0]);
        if (target is null)
        {
            Log.Info($"{args[0]}不存在。");
        }
        else
        {
            Game1.server.kick(target.UniqueMultiplayerID);
            Game1.otherFarmers.Remove(target.UniqueMultiplayerID);
            Log.Info($"{target.Name}已被踢出。");
        }
    }

    private void KickAllPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        foreach (var (id, farmer) in Game1.otherFarmers)
        {
            Game1.server.kick(id);
            Log.Info($"{farmer.Name}已被踢出。");
        }
    }

    private void AccessInventory(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var farmer = Game1.getOnlineFarmers().FirstOrDefault(x => x.Name == args[0]);
        if (farmer is null)
        {
            Log.Info($"{args[0]}不存在，无法访问该玩家的背包。");
        }
        else
        {
            Log.Alert($"{farmer.Name}的背包中有：");
            foreach (var item in farmer.Items)
            {
                if (item is null) continue;
                Log.Info($"{item.Stack}\t{item.DisplayName}");
            }
        }
    }

    private void SetServerMode(string command, string[] args)
    {
        if (Game1.IsClient) return;

        if (args.Length < 1)
        {
            Log.Error("该命令需要参数，可选的参数为：<offline>、<friends>和<invite>。");
            return;
        }

        var mode = args[0];
        if (!new HashSet<string> { "offline", "friends", "invite" }.Contains(mode))
        {
            Log.Error("模式输入错误，可用的模式为：<offline>、<friends>和<invite>。");
            return;
        }
        Game1.options.setServerMode(mode);
    }
}