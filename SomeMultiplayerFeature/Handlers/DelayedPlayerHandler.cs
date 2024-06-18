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
        if (!Context.IsMainPlayer || !config.EnableKickDelayedPlayer)
        {
            delayedPlayers.Clear();
            return;
        }

        foreach (var farmer in Game1.getOnlineFarmers())
        {
            if (farmer.IsMainPlayer) continue;

            if (Game1.server.getPingToClient(farmer.UniqueMultiplayerID) >= 100)
                delayedPlayers.TryAdd(farmer.UniqueMultiplayerID, 1);
            else
                delayedPlayers.Remove(farmer.UniqueMultiplayerID);
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