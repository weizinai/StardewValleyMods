using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace LazyMod.Framework.Hud;

public abstract class MiningHud
{
    protected readonly ModConfig Config;
    protected Rectangle Bounds = new(0, 0, 64, 64);
    protected Rectangle InnerBounds = new(16, 0, 32, 32);
    protected Texture2D? Texture;

    protected MiningHud(ModConfig config)
    {
        Config = config;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, Color.White);
    }

    public virtual bool IsShowing()
    {
        return false;
    }

    public void SetYPosition(int y)
    {
        Bounds.Y = y;
        InnerBounds.Y = y + 16;
    }

    protected bool GetBuildingLayerInfo(int targetIndex)
    {
        var location = Game1.currentLocation;
        if (location is not MineShaft mineShaft) return false;

        var buildingLayer = mineShaft.Map.GetLayer("Buildings");
        for (var i = 0; i < buildingLayer.LayerWidth; i++)
        for (var j = 0; j < buildingLayer.LayerHeight; j++)
        {
            var index = mineShaft.getTileIndexAt(i, j, "Buildings");
            if (index == targetIndex) return true;
        }

        return false;
    }

    protected string GetStringFromDictionary(Dictionary<string, int> dictionary)
    {
        var stringBuilder = new StringBuilder();
        foreach (var (key, value) in dictionary)
            stringBuilder.AppendLine($"{key}: {value}");
        stringBuilder.Remove(stringBuilder.Length - 1, 1);

        return stringBuilder.ToString();
    }
}