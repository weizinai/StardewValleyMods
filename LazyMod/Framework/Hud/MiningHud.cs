using System.Text;
using Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Hud;

public class MiningHud
{
    private readonly RootElement hud;
    private bool hasGetMineralInfo;
    private readonly Dictionary<string, int> mineralInfo = new();
    private bool hasGetMonsterInfo;
    private readonly Dictionary<string, int> monsterInfo = new();
    
    public MiningHud(ModConfig config)
    {
        hud = new RootElement();
        
        var ladderHud = new ImageWithBackground(GenerateBackground(0), Game1.temporaryContent.Load<Texture2D>("Maps/Mines/mine_desert"),
            new Rectangle(208, 160, 16, 16))
        {
            CheckHidden = () => !(config.ShowLadderInfo && GetBuildingLayerInfo(173))
        };
        var shaftHud = new ImageWithBackground(GenerateBackground(1), Game1.temporaryContent.Load<Texture2D>("Maps/Mines/mine_desert"),
            new Rectangle(224, 160, 16, 16))
        {
            CheckHidden = () => !(config.ShowShaftInfo && GetBuildingLayerInfo(174))
        };
        var monsterHud = new ImageWithBackground(GenerateBackground(2), Game1.temporaryContent.Load<Texture2D>("Characters/Monsters/Green Slime"),
            new Rectangle(2, 268, 12, 10))
        {
            CheckHidden = () => !(config.ShowMonsterInfo && GetMonsters().Any()),
            OnHover = spriteBatch =>
            {
                if (!hud.Children[2].Hover)
                {
                    hasGetMonsterInfo = false;
                    return;
                }

                if (!hasGetMonsterInfo)
                {
                    GetMonsterInfo();
                    hasGetMonsterInfo = true;
                }
                var monsterInfoString = GetStringFromDictionary(monsterInfo);
                IClickableMenu.drawHoverText(spriteBatch, monsterInfoString, Game1.smallFont);
            }
        };
        var mineralHud = new ImageWithBackground(GenerateBackground(3), Game1.temporaryContent.Load<Texture2D>("TileSheets/tools"),
            new Rectangle(193, 128, 15, 15))
        {
            CheckHidden = () => !(config.ShowMineralInfo && GetMinerals().Any()),
            OnHover = spriteBatch =>
            {
                if (!hud.Children[3].Hover)
                {
                    hasGetMineralInfo = false;
                    return;
                }

                if (!hasGetMineralInfo)
                {
                    GetMineralInfo();
                    hasGetMineralInfo = true;
                }
        
                var mineralInfoString = GetStringFromDictionary(mineralInfo);
                IClickableMenu.drawHoverText(spriteBatch, mineralInfoString, Game1.smallFont);
            } 
        };
        
        hud.AddChild(ladderHud, shaftHud, monsterHud, mineralHud);
    }

    public void Update()
    {
        if (Game1.player.currentLocation is not MineShaft) return;

        var j = 0;
        foreach (var element in hud.Children.Where(element => !element.IsHidden())) element.Position = GetDestinationRectangle(j++).Location.ToVector2();

        hud.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Game1.player.currentLocation is not MineShaft) return;

        hud.Draw(spriteBatch);

        hud.Children[2].PerformHoverAction(spriteBatch);
        hud.Children[3].PerformHoverAction(spriteBatch);
    }

    private Image GenerateBackground(int index)
    {
        var texture = Game1.temporaryContent.Load<Texture2D>("Maps/MenuTiles");
        var destinationRectangle = GetDestinationRectangle(index);
        var sourceRectangle = new Rectangle(0, 256, 64, 64);
        return new Image(texture, destinationRectangle, sourceRectangle);
    }

    private Rectangle GetDestinationRectangle(int index)
    {
        return new Rectangle(0, 88 + 72 * index, 64, 64);
    }
    
    private bool GetBuildingLayerInfo(int targetIndex)
    {
        var location = Game1.currentLocation;
        if (location is not MineShaft mineShaft) return false;

        var buildingLayer = mineShaft.Map.GetLayer("Buildings");
        for (var i = 0; i < buildingLayer.LayerWidth; i++)
        {
            for (var j = 0; j < buildingLayer.LayerHeight; j++)
            {
                var index = mineShaft.getTileIndexAt(i, j, "Buildings");
                if (index == targetIndex) return true;
            }
        }

        return false;
    }
    
    private List<Monster> GetMonsters()
    {
        var location = Game1.currentLocation;
        if (location is not MineShaft mineShaft) return new List<Monster>();

        var monsters = mineShaft.characters.OfType<Monster>().ToList();
        return monsters;
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

    private string GetStringFromDictionary(Dictionary<string, int> dictionary)
    {
        var stringBuilder = new StringBuilder();
        foreach (var (key, value) in dictionary)
            stringBuilder.AppendLine($"{key}: {value}");
        stringBuilder.Remove(stringBuilder.Length - 1, 1);

        return stringBuilder.ToString();
    }
}