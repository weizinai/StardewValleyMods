using Common;
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
            Log.Info("-- 开始踢出未准备好的玩家 --");
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                if (farmer.isUnclaimedFarmhand)
                {
                    Log.Info($"{farmer.Name}未准备好，已被踢出");
                    Game1.server.kick(farmer.UniqueMultiplayerID);
                }
            }
            Log.Info("-- 结束踢出未准备好的玩家 --");
        }
    }
}