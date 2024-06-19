using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature.Handlers;

/// <summary>
/// 实现与踢出未准备玩家功能相关的逻辑
/// </summary>
/// <remarks>该功能仅主机端可用</remarks>
internal class UnreadyPlayerHandler : BaseHandler
{
    private readonly HashSet<long> unreadyPlayers = new();

    public UnreadyPlayerHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        Helper.Events.Input.ButtonsChanged += OnButtonChanged;
        Helper.Events.Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
    }

    // 如果当前玩家不是房主，则检测该玩家是否准备好，若未准备好，则向房主发送消息
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // 如果当前没有玩家在线或者当前玩家是主机端，则返回
        if (!Context.HasRemotePlayers || Context.IsMainPlayer) return;

        Helper.Multiplayer.SendMessage(Game1.activeClickableMenu is not ReadyCheckDialog ? "Unready" : "Ready", "ReadyCheck",
            new[] { "weizinai.SomeMultiplayerFeature" }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    // 当按下设置的快捷键时，若当前玩家是主机端，则踢出所有未准备好的玩家并显示信息
    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        // 如果该功能未启用，则放回
        if (!Config.EnableKickUnreadyPlayer) return;
        
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        if (Config.KickUnreadyPlayerKey.JustPressed())
        {
            Log.Info("-- 开始踢出玩家 --");
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                var id = farmer.UniqueMultiplayerID;
                if (Game1.Multiplayer.isDisconnecting(id))
                {
                    Game1.otherFarmers.Remove(id);
                    unreadyPlayers.Remove(id);
                    Log.Alert($"{farmer.Name}已断开连接，但游戏内依然存在，已被移除。");
                }
            }

            foreach (var player in unreadyPlayers)
            {
                Game1.server.kick(player);
                Log.Info($"{Game1.getFarmer(player).Name}未断开连接，但其未准备好，已被踢出。");
            }

            unreadyPlayers.Clear();
            Log.Info("-- 结束踢出玩家 --");
        }
    }

    // 如果有玩家退出，则将其移出'unreadyPlayers'
    private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
    {
        // 如果当前玩家不是主机端，则返回
        if (!Context.IsMainPlayer) return;
        
        unreadyPlayers.Remove(e.Peer.PlayerID);
    }

    // 如果当前玩家是主机端，则接受来自其他玩家的消息，若该玩家未准备好，则将其加入'unreadyPlayers'
    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!Config.EnableKickUnreadyPlayer) return;
        
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        if (e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "ReadyCheck" })
        {
            var message = e.ReadAs<string>();
            if (message is "Unready")
                unreadyPlayers.Add(e.FromPlayerID);
            else
                unreadyPlayers.Remove(e.FromPlayerID);
        }
    }
}