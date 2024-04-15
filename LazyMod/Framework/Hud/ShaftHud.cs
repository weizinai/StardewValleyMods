using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace LazyMod.Framework.Hud;

public class ShaftHud : MiningHud
{
    public ShaftHud(ModConfig config) : base(config)
    {
        Texture = Game1.content.Load<Texture2D>("Maps/Mines/mine_desert");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.Draw(Texture, InnerBounds, new Rectangle(224, 160, 16, 16), Color.White);
    }

    public override bool IsShowing()
    {
        return Config.ShowShaftInfo && GetBuildingLayerInfo(174);
    }
}