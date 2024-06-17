using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Framework.UI;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class PlayerCountHandler
{
    private readonly ModConfig config;
    private readonly Button playerCountButton = new(new Point(64, 64), "");

    public PlayerCountHandler(ModConfig config)
    {
        this.config = config;
    }

    public void OnSecondUpdateTicked()
    {
        if (!config.ShowPlayerCount) return;
        playerCountButton.name = Game1.getAllFarmers().Count() + "个玩家在线";
    }

    public void OnRendered(SpriteBatch spriteBatch)
    {
        if (!config.ShowPlayerCount) return;
        playerCountButton.Draw(spriteBatch);
    }
}