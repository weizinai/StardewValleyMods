using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class DelayedPlayerHandler
{
    private readonly ModConfig config;
    
    private readonly Dictionary<long, int> delayedPlayers = new();

    public DelayedPlayerHandler(ModConfig config)
    {
        this.config = config;
    }

    public void OnSecondUpdateTicked()
    {
        if (!Context.IsMainPlayer || !config.KickDelayedPlayer)
        {
            delayedPlayers.Clear();
            return;
        }

        foreach (var (id, _) in Game1.otherFarmers)
        {
            if (Game1.server.getPingToClient(id) >= 100)
                delayedPlayers.TryAdd(id, 1);
            else
                delayedPlayers.Remove(id);  
        }

        foreach (var (id, count) in delayedPlayers)
        {
            if (count >= 5)
            {
                Log.Info($"{Game1.otherFarmers[id].Name}的延迟过高，已自动将其踢出。");
                Game1.server.kick(id);
            }
        }
    }
}