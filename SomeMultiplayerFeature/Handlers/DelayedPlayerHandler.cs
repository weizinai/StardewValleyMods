using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class DelayedPlayerHandler : BaseHandler
{
    private bool isBusy;
    private int coolDown;

    public DelayedPlayerHandler(IModHelper helper, ModConfig config) 
        : base(helper, config) {}

    public override void Init()
    {
        Helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
        Helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
    }

    // 每5秒显示延迟超过100ms的玩家中延迟最高的玩家
    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Context.IsWorldReady || !Config.EnableKickDelayedPlayer) return;

        coolDown++;
        if (coolDown < 5) return;
        
        var playerPing = new Dictionary<string, float>();
        foreach (var farmer in Game1.getOnlineFarmers())
        {
            if (farmer.IsMainPlayer) continue;
            
            var ping = Game1.server.getPingToClient(farmer.UniqueMultiplayerID);
            if (ping >= 100) playerPing.Add(farmer.Name, ping);
        }

        if (playerPing.Any())
        {
            Log.Alert($"{playerPing.MaxBy(x => x.Value).Key}的延迟超过100ms，且其延迟最高。");
        }
        
        coolDown = 0;
    }

    // 当某个玩家加入游戏后，若超过一半的玩家延迟超过100ms，则发送信息
    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Context.IsWorldReady || !Config.EnableKickDelayedPlayer) return;
        
        var delayedPlayerCount = Game1.getOnlineFarmers().Count(farmer => Game1.server.getPingToClient(farmer.UniqueMultiplayerID) >= 100);

        if (delayedPlayerCount >= Game1.getOnlineFarmers().Count / 2 && !isBusy)
        {
            isBusy = true;
            Log.Alert($"{Game1.getFarmer(e.Peer.PlayerID).Name}加入游戏后，超过一半的玩家延迟超过100ms。");
        }
        else
        {
            isBusy = false;
        }
    }
}