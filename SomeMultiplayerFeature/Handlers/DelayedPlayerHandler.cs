using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class DelayedPlayerHandler : BaseHandler
{
    private int cooldown;

    public DelayedPlayerHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        Helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // 如果功能未启用，则返回
        if (!Config.ShowDelayedPlayer) return;

        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        cooldown++;
        
        if (cooldown >= Config.ShowInterval)
        {
            cooldown = 0;
            
            var playerPing = new Dictionary<string, float>();
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                if (farmer.IsMainPlayer) continue;

                var ping = Game1.server.getPingToClient(farmer.UniqueMultiplayerID);
                if (ping >= 100) playerPing.Add(farmer.Name, ping);
            }

            if (playerPing.Any()) Log.Alert($"{playerPing.MaxBy(x => x.Value).Key}的延迟超过100ms，且其延迟最高。");
        }
    }
}