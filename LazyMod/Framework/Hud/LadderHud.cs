using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace LazyMod.Framework.Hud;

public class LadderHud : MiningHud
{
    public LadderHud(ModConfig config) : base(config)
    {
        Texture = Game1.content.Load<Texture2D>("Maps/Mines/mine_desert");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.Draw(Texture, InnerBounds, new Rectangle(208, 160, 16, 16), Color.White);
    }

    public override bool IsShowing()
    {
        return Config.ShowLadderInfo && GetBuildingLayerInfo(173);
    }
}