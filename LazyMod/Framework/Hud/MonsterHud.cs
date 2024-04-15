using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace LazyMod.Framework.Hud;

public class MonsterHud : MiningHud
{
    private bool hasGetInfo;
    private readonly Dictionary<string, int> monsterInfo = new();
    
    public MonsterHud(ModConfig config) : base(config)
    {
        Texture = Game1.content.Load<Texture2D>("Characters/Monsters/Green Slime");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.Draw(Texture, InnerBounds, new Rectangle(2, 268, 12, 10), Color.White);
        PerformHoverAction(spriteBatch);
    }

    public override bool IsShowing()
    {
        return Config.ShowMonsterInfo && GetMonsters().Any();
    }

    private void GetMonsterInfo()
    {
        var monsters = GetMonsters();
        monsterInfo.Clear();
        foreach (var monster in monsters)
        {
            if (!monsterInfo.TryAdd(monster.displayName, 1))
                monsterInfo[monster.displayName]++;
        }
    }

    private List<Monster> GetMonsters()
    {
        var location = Game1.currentLocation;
        if (location is not MineShaft mineShaft) return new List<Monster>();

        var monsters = mineShaft.characters.OfType<Monster>().ToList();
        return monsters;
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
            GetMonsterInfo();
            hasGetInfo = true;
        }
        var monsterInfoString = GetStringFromDictionary(monsterInfo);
        IClickableMenu.drawHoverText(spriteBatch, monsterInfoString, Game1.smallFont);
    }
}