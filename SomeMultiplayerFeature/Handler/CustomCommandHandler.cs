using System.Collections.Generic;
using System.Linq;
using ConsoleTables;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class CustomCommandHandler : BaseHandler
{
    private const string BannedPlayerPath = "assets/BannedPlayer.json";
    private readonly Dictionary<long, string>? bannedPlayers;

    public CustomCommandHandler(IModHelper helper) : base(helper)
    {
        this.bannedPlayers = helper.Data.ReadJsonFile<Dictionary<long, string>>(BannedPlayerPath);
        if (this.bannedPlayers is null)
        {
            this.bannedPlayers = new Dictionary<long, string>();
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
            Logger.Warn($"无法获取id为{id}的玩家");
            return;
        }

        if (this.bannedPlayers!.ContainsKey(id))
        {
            Game1.server.kick(id);
            Logger.Alert($"{player.Name}在黑名单中，已被踢出。");
        }
    }

    private void BanPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var target = Game1.getAllFarmhands().Where(x => x.Name == args[0]).ToArray();

        if (!target.Any())
        {
            Logger.Error($"不存在名字为{args[0]}的玩家");
            return;
        }

        foreach (var farmer in target)
        {
            var id = farmer.UniqueMultiplayerID;
            var name = farmer.Name;

            if (this.bannedPlayers!.ContainsKey(id))
            {
                Logger.Info($"{name}已经在黑名单中。");
            }
            else
            {
                this.bannedPlayers.Add(id, name);
                Game1.server.kick(id);
                Logger.Info($"{name}被加入黑名单。");
            }
        }

        this.Helper.Data.WriteJsonFile(BannedPlayerPath, this.bannedPlayers);
    }

    private void UnbanPlayer(string command, string[] args)
    {
        // 如果当前不是联机模式或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        var target = this.bannedPlayers!.Where(x => x.Value == args[0]).ToList();

        if (!target.Any())
        {
            Logger.Info($"{args[0]}不在黑名单中。");
        }

        foreach (var (id, name) in target)
        {
            this.bannedPlayers!.Remove(id);
            Logger.Info($"{name}被移出黑名单。");
        }

        this.Helper.Data.WriteJsonFile(BannedPlayerPath, this.bannedPlayers);
    }

    private void PingPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var farmersData = new ConsoleTable("名字", "IP", "延迟");

        var otherFarmer = Game1.otherFarmers
            .OrderByDescending(x => (int)Game1.server.getPingToClient(x.Key));

        foreach (var (id, farmer) in otherFarmer)
        {
            farmersData.AddRow(
                farmer.Name,
                Game1.Multiplayer.getUserName(farmer.UniqueMultiplayerID),
                $"{(int)Game1.server.getPingToClient(id)}ms"
            );
        }

        Logger.Info($"\n{farmersData.ToMarkDownString()}");
    }

    private void ListPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        var farmersData = new ConsoleTable("名字", "状态", "地点", "IP", "总在线时间", "上次在线时间");

        var allFarmhands = Game1.getAllFarmhands()
            .Where(x => x.Name != "")
            .OrderByDescending(x => x.millisecondsPlayed);

        foreach (var farmer in allFarmhands)
        {
            var isOnline = Game1.player.team.playerIsOnline(farmer.UniqueMultiplayerID);
            farmersData.AddRow(
                farmer.Name,
                isOnline ? "在线" : "离线",
                isOnline ? farmer.currentLocation.DisplayName : "无",
                isOnline ? Game1.Multiplayer.getUserName(farmer.UniqueMultiplayerID) : "无",
                Utility.getHoursMinutesStringFromMilliseconds(farmer.millisecondsPlayed),
                WorldDate.ForDaysPlayed(farmer.disconnectDay.Value).Localize()
            );
        }

        Logger.Info($"总人数：{farmersData.Rows.Count}\n{farmersData.ToMarkDownString()}");
    }

    private void KickPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var target = Game1.getOnlineFarmers().FirstOrDefault(x => x.Name == args[0]);

        if (target is null)
        {
            Logger.Info($"{args[0]}不存在。");
        }
        else
        {
            Game1.server.kick(target.UniqueMultiplayerID);
            Game1.otherFarmers.Remove(target.UniqueMultiplayerID);
            Logger.Info($"{target.Name}已被踢出。");
        }
    }

    private void KickAllPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        foreach (var (id, farmer) in Game1.otherFarmers)
        {
            Game1.server.kick(id);
            Logger.Info($"{farmer.Name}已被踢出。");
        }
    }

    private void AccessInventory(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var farmer = Game1.getOnlineFarmers().FirstOrDefault(x => x.Name == args[0]);
        if (farmer is null)
        {
            Logger.Info($"{args[0]}不存在，无法访问该玩家的背包。");
        }
        else
        {
            var inventoryData = new ConsoleTable("物品", "数量");

            foreach (var item in farmer.Items.OrderByDescending(item => item?.Stack))
            {
                if (item is null) continue;
                inventoryData.AddRow(
                    item.DisplayName,
                    item.Stack
                );
            }

            Logger.Info($"{farmer.Name}的背包有：\n{inventoryData.ToMarkDownString()}");
        }
    }

    private void SetServerMode(string command, string[] args)
    {
        if (Game1.IsClient) return;

        if (args.Length < 1)
        {
            Logger.Error("该命令需要参数，可选的参数为：<offline>、<friends>和<invite>。");
            return;
        }

        var mode = args[0];
        if (!new HashSet<string> { "offline", "friends", "invite" }.Contains(mode))
        {
            Logger.Error("模式输入错误，可用的模式为：<offline>、<friends>和<invite>。");
            return;
        }
        Game1.options.setServerMode(mode);
    }
}