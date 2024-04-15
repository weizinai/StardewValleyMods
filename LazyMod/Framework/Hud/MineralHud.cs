using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Hud;

public class MineralHud : MiningHud
{
    private bool hasGetInfo;
    private readonly Dictionary<string, int> mineralInfo = new();
    
    public MineralHud(ModConfig config) : base(config)
    {
        Texture = Game1.content.Load<Texture2D>("TileSheets/tools");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.Draw(Texture, InnerBounds, new Rectangle(193, 128, 15, 15), Color.White);
        PerformHoverAction(spriteBatch);
    }

    public override bool IsShowing()
    {
        return Config.ShowMineralInfo && GetMinerals().Any();
    }

    private void PerformHoverAction(SpriteBatch spriteBatch)
    {
        var mousePosition = Game1.getMousePosition();
        if (!Bounds.Contains(mousePosition))
        {
            hasGetInfo = false;
            return;
        }

        if (!hasGetInfo)
        {
            GetMineralInfo();
            hasGetInfo = true;
        }
        
        var mineralInfoString = GetStringFromDictionary(mineralInfo);
        IClickableMenu.drawHoverText(spriteBatch, mineralInfoString, Game1.smallFont);
    }
    
    private void GetMineralInfo()
    {
        var minerals = GetMinerals();
        mineralInfo.Clear();
        foreach (var mineral in minerals)
        {
            if (!mineralInfo.TryAdd(mineral.DisplayName, 1))
                mineralInfo[mineral.DisplayName]++;
        }
    }
    
    private List<SObject> GetMinerals()
    {
        var location = Game1.currentLocation;
        if (location is not MineShaft mineShaft) return new List<SObject>();
        var minerals = mineShaft.Objects.Values.ToList();
        return minerals;
    }
}