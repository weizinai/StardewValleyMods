using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal abstract class BaseAutomationHandler : IAutomationHandler
{
    protected readonly ModConfig Config;

    protected BaseAutomationHandler(ModConfig config)
    {
        this.Config = config;
    }

    public abstract void Apply(Item item, Farmer player, GameLocation location);

    protected void ForEachTile(int range, Func<Vector2, bool> action)
    {
        var grid = TileHelper.GetTileGrid(range);
        foreach (var tile in grid) if (!action(tile)) break;
    }

    protected Rectangle GetTileBoundingBox(Vector2 tile)
    {
        var position = PositionHelper.GetAbsolutePositionFromTilePosition(tile);
        return new Rectangle((int)position.X, (int)position.Y, Game1.tileSize, Game1.tileSize);
    }

    protected void UseToolOnTile(GameLocation location, Farmer player, Tool tool, Vector2 tile)
    {
        var position = PositionHelper.GetAbsolutePositionFromTilePosition(tile, true);
        tool.swingTicker++;
        tool.DoFunction(location, (int)position.X, (int)position.Y, 1, player);
    }

    protected FarmAnimal? GetBestHarvestableFarmAnimal(Tool tool, Vector2 tile, IEnumerable<FarmAnimal> animals)
    {
        var animal = Utility.GetBestHarvestableFarmAnimal(animals, tool, this.GetTileBoundingBox(tile));
        if (animal?.currentProduce.Value is null || animal.isBaby() || !animal.CanGetProduceWithTool(tool))
            return null;

        return animal;
    }

    protected bool CanAddItemToInventory(Item item)
    {
        return Game1.player.freeSpotsInInventory() > 1 || Game1.player.Items.Any(item.canStackWith);
    }

    protected void ConsumeItem(Farmer player, Item item)
    {
        item.Stack--;
        if (item.Stack <= 0) player.removeItemFromInventory(item);
    }

    protected void CheckTileAction(GameLocation location, Farmer player, Vector2 tile)
    {
        location.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, player);
    }

    protected void PlaceObjectAction(SObject obj, Vector2 tile, Farmer player, GameLocation location)
    {
        var position = PositionHelper.GetAbsolutePositionFromTilePosition(tile, true);
        if (obj.placementAction(location, (int)position.X, (int)position.Y, player))
            player.reduceActiveItemByOne();
    }
}