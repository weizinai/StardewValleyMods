using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoAnimal : Automate
{
    private readonly ModConfig config;

    public AutoAnimal(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;

        // 自动抚摸动物
        if (config.AutoPetAnimal) AutoPetAnimal(location, player);
        // 自动挤奶
        if (config.AutoMilkAnimal && (tool is MilkPail || config.FindMilkPailFromInventory)) AutoMilkAnimal(location, player);
        // 自动剪毛
        if (config.AutoShearsAnimal && (tool is Shears || config.FindShearsFromInventory)) AutoShearsAnimal(location, player);
        // 自动打开栅栏门
        if (config.AutoOpenFenceGate) AutoOpenFenceGate(location, player);
        // 自动抚摸宠物
        if (config.AutoPetPet) AutoPetPet(location, player);
    }

    // 自动抚摸动物
    private void AutoPetAnimal(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoPetAnimalRange).ToList();

        var animals = location.animals.Values;
        foreach (var animal in animals)
        {
            foreach (var tile in grid)
            {
                if (animal.GetCursorPetBoundingBox().Intersects(GetTileBoundingBox(tile)) && !animal.wasPet.Value)
                    animal.pet(player);
            }
        }
    }

    // 自动挤奶
    private void AutoMilkAnimal(GameLocation location, Farmer player)
    {
        var milkPail = FindToolFromInventory<MilkPail>();
        if (milkPail is null) return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoMilkAnimalRange);
        foreach (var tile in grid)
        {
            var animal = GetBestHarvestableFarmAnimal(location, milkPail, tile);
            if (animal is null)
                break;
            UseToolOnTile(location, player, milkPail, tile);
        }
    }

    // 自动剪毛
    private void AutoShearsAnimal(GameLocation location, Farmer player)
    {
        var shears = FindToolFromInventory<Shears>();
        if (shears is null)
            return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoShearsAnimalRange);
        foreach (var tile in grid)
        {
            var animal = GetBestHarvestableFarmAnimal(location, shears, tile);
            if (animal is null)
                break;
            shears.animal = animal;
            UseToolOnTile(location, player, shears, tile);
        }
    }

    // 自动打开动物门
    public static void AutoToggleAnimalDoor(bool isOpen)
    {
        if (isOpen && (Game1.isRaining || Game1.IsWinter))
            return;

        var buildableLocations = GetBuildableLocation().ToList();
        foreach (var location in buildableLocations)
        {
            foreach (var building in location.buildings)
            {
                // 如果该建筑没有动物门，或者动物门已经是目标状态，则跳过
                if (building.animalDoor is null || building.animalDoorOpen.Value == isOpen) continue;
                // 遍历所有的动物,将不在家的动物传送回家
                foreach (var animal in location.Animals.Values.Where(animal => !animal.IsHome && animal.home == building)) animal.warpHome();
                // 切换动物门状态
                building.ToggleAnimalDoor(Game1.player);
            }
        }
    }

    // 自动打开栅栏门
    private void AutoOpenFenceGate(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoOpenFenceGateRange + 2).ToList();
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not Fence fence || !fence.isGate.Value)
                continue;

            var distance = GetDistance(origin, tile);
            if (distance <= config.AutoOpenFenceGateRange && fence.gatePosition.Value == 0)
            {
                fence.toggleGate(player, true);
            }
            else if (distance > config.AutoOpenFenceGateRange + 1 && fence.gatePosition.Value != 0)
            {
                fence.toggleGate(player, false);
            }
        }
    }

    // 自动抚摸宠物
    private void AutoPetPet(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoPetAnimalRange).ToList();

        var pets = location.characters.OfType<Pet>();
        foreach (var pet in pets)
        {
            foreach (var tile in grid)
            {
                if (pet.GetBoundingBox().Intersects(GetTileBoundingBox(tile)) &&
                    (!pet.lastPetDay.TryGetValue(player.UniqueMultiplayerID, out var lastPetDay) || lastPetDay != Game1.Date.TotalDays))
                    pet.checkAction(player, location);
            }
        }
    }

    private FarmAnimal? GetBestHarvestableFarmAnimal(GameLocation location, Tool tool, Vector2 tile)
    {
        var animal = Utility.GetBestHarvestableFarmAnimal(location.Animals.Values, tool, GetTileBoundingBox(tile));
        if (animal?.currentProduce.Value is null || animal.isBaby() || !animal.CanGetProduceWithTool(tool))
            return null;

        return animal;
    }

    private static IEnumerable<GameLocation> GetBuildableLocation()
    {
        return Game1.locations.Where(location => location.IsBuildableLocation());
    }

    private int GetDistance(Vector2 origin, Vector2 tile)
    {
        return Math.Max(Math.Abs((int)(origin.X - tile.X)), Math.Abs((int)(origin.Y - tile.Y)));
    }
}