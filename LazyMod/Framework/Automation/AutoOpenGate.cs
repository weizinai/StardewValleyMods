using Microsoft.Xna.Framework;
using StardewValley;

namespace LazyMod.Framework.Automation;

public class AutoOpenGate : Automate
{
    private readonly ModConfig config;

    public AutoOpenGate(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        if (config.AutoOpenFenceGate)
            AutoOpenFenceGate(location, player);
    }

    public static void AutoOpenAnimalDoor()
    {
        if (Game1.isRaining || Game1.isSnowing || Game1.IsWinter)
            return;

        var buildableLocations = GetBuildableLocation().ToList();
        foreach (var location in buildableLocations)
        {
            foreach (var building in location.buildings)
            {
                if (building.animalDoor is null)
                    break;
                if (!building.animalDoorOpen.Value)
                    building.ToggleAnimalDoor(Game1.player);
            }
        }
    }

    public static void AutoCloseAnimalDoor()
    {
        var buildableLocations = GetBuildableLocation().ToList();
        foreach (var location in buildableLocations)
        {
            foreach (var building in location.buildings)
            {
                if (building.animalDoor is null)
                    break;
                if (building.animalDoorOpen.Value)
                    building.ToggleAnimalDoor(Game1.player);
            }
        }
    }

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

    private int GetDistance(Vector2 origin, Vector2 tile)
    {
        return Math.Max(Math.Abs((int)(origin.X - tile.X)), Math.Abs((int)(origin.Y - tile.Y)));
    }

    private static IEnumerable<GameLocation> GetBuildableLocation()
    {
        return Game1.locations.Where(location => location.IsBuildableLocation());
    }
}