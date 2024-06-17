using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class UnreadyPlayerHandler
{
    private readonly ModConfig config;

    public UnreadyPlayerHandler(ModConfig config)
    {
        this.config = config;
    }

    public void OnButtonChanged()
    {
        if (!Context.IsMainPlayer || !Context.IsWorldReady) return;

        if (config.KickUnreadyPlayerKey.JustPressed())
        {
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                if (farmer.isUnclaimedFarmhand)
                    Game1.server.kick(farmer.UniqueMultiplayerID);
            }
        }
    }
}