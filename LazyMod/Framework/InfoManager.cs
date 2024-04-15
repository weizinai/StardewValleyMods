using LazyMod.Framework.Info;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace LazyMod.Framework;

public class InfoManager
{
    private readonly ModConfig config;
    private readonly MiningInfo miningInfo;

    public InfoManager(ModConfig config)
    {
        this.config = config;
        miningInfo = new MiningInfo(config);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        miningInfo.Draw(spriteBatch);
    }
}