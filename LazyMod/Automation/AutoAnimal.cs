using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Automation;

internal class AutoAnimal : Automate
{
    public AutoAnimal(ModConfig config) : base(config)
    {
    }

    public override void Apply(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动挤奶
        if (this.Config.AutoMilkAnimal.IsEnable && (tool is MilkPail || this.Config.AutoMilkAnimal.FindToolFromInventory)) this.AutoMilkAnimal(location, player);
        // 自动剪毛
        if (this.Config.AutoShearsAnimal.IsEnable && (tool is Shears || this.Config.AutoShearsAnimal.FindToolFromInventory)) this.AutoShearsAnimal(location, player);
        // 自动喂食动物饼干
        if (this.Config.AutoFeedAnimalCracker.IsEnable && item?.QualifiedItemId is "(O)GoldenAnimalCracker") this.AutoFeedAnimalCracker(location, player);
        // 自动打开栅栏门
        if (this.Config.AutoOpenFenceGate.IsEnable) this.AutoOpenFenceGate(location, player);
    }

    // 自动挤奶
    private void AutoMilkAnimal(GameLocation location, Farmer player)
    {
        if (player.Stamina <= this.Config.AutoMilkAnimal.StopStamina) return;
        if (player.freeSpotsInInventory() < 1) return;

        var milkPail = ToolHelper.FindToolFromInventory<MilkPail>();
        if (milkPail is null) return;

        var grid = this.GetTileGrid(this.Config.AutoMilkAnimal.Range);
        foreach (var tile in grid)
        {
            var animal = this.GetBestHarvestableFarmAnimal(location, milkPail, tile);
            if (animal is null) continue;
            milkPail.animal = animal;
            this.UseToolOnTile(location, player, milkPail, tile);
        }
    }

    // 自动剪毛
    private void AutoShearsAnimal(GameLocation location, Farmer player)
    {
        if (player.Stamina <= this.Config.AutoShearsAnimal.StopStamina) return;
        if (player.freeSpotsInInventory() < 1) return;

        var shears = ToolHelper.FindToolFromInventory<Shears>();
        if (shears is null)
            return;

        var grid = this.GetTileGrid(this.Config.AutoShearsAnimal.Range);
        foreach (var tile in grid)
        {
            var animal = this.GetBestHarvestableFarmAnimal(location, shears, tile);
            if (animal is null) continue;
            shears.animal = animal;
            this.UseToolOnTile(location, player, shears, tile);
        }
    }

    // 自动喂食动物饼干
    private void AutoFeedAnimalCracker(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoFeedAnimalCracker.Range);
        var animals = location.animals.Values;
        foreach (var animal in animals)
            foreach (var tile in grid)
                if (this.CanFeedAnimalCracker(tile, animal))
                    this.FeedAnimalCracker(player, animal);
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
        var grid = this.GetTileGrid(this.Config.AutoOpenFenceGate.Range + 2);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not Fence fence || !fence.isGate.Value)
                continue;

            var distance = this.GetDistance(player.Tile, tile);
            if (distance <= this.Config.AutoOpenFenceGate.Range && fence.gatePosition.Value == 0)
            {
                fence.toggleGate(player, true);
            }
            else if (distance > this.Config.AutoOpenFenceGate.Range + 1 && fence.gatePosition.Value != 0)
            {
                fence.toggleGate(player, false);
            }
        }
    }

    private FarmAnimal? GetBestHarvestableFarmAnimal(GameLocation location, Tool tool, Vector2 tile)
    {
        var animal = Utility.GetBestHarvestableFarmAnimal(location.Animals.Values, tool, this.GetTileBoundingBox(tile));
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

    private bool CanFeedAnimalCracker(Vector2 tile, FarmAnimal animal)
    {
        return animal.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               !animal.hasEatenAnimalCracker.Value &&
               (animal.GetAnimalData()?.CanEatGoldenCrackers ?? false);
    }

    private void FeedAnimalCracker(Farmer player, FarmAnimal animal)
    {
        animal.hasEatenAnimalCracker.Value = true;
        Game1.playSound("give_gift");
        animal.doEmote(56);
        player.reduceActiveItemByOne();
    }
}