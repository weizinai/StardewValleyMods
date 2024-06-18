using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class DelayedPlayerHandler : BaseHandler
{
    private readonly Dictionary<long, int> delayedPlayers = new();

    public DelayedPlayerHandler(IModHelper helper, ModConfig config) 
        : base(helper, config) {}

    public override void Init()
    {
        Helper.Events.GameLoop.OneSecondUpdateTicked += OnSecondUpdateTicked;
    }

    public void OnSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Config.EnableKickDelayedPlayer)
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