using LazyMod.Framework.Hud;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace LazyMod.Framework.Info;

public class MiningInfo
{
    private readonly List<MiningHud> miningHuds = new();
    
    public MiningInfo(ModConfig config)
    {
        miningHuds.AddRange(new MiningHud[]
        {
            new LadderHud(config),
            new ShaftHud(config),
            new MonsterHud(config),
            new MineralHud(config),
        });
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Game1.currentLocation is not MineShaft or VolcanoDungeon) return;

        var i = 0;
        foreach (var miningHud in miningHuds.Where(miningHud => miningHud.IsShowing()))
        {
            miningHud.SetYPosition(88 + i++ * 72);
            miningHud.Draw(spriteBatch);
        }
    }
}