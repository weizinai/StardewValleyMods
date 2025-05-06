using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class TillDirtHandler : BaseAutomationHandler
{
    public TillDirtHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var hoe = ToolHelper.GetTool<Hoe>(this.Config.AutoTillDirt.FindToolFromInventory);
        if (hoe is null) return;

        this.ForEachTile(this.Config.AutoTillDirt.Range, tile =>
        {
            if (player.Stamina <= this.Config.AutoTillDirt.StopStamina) return false;
            if (this.CanTillDirt(tile, location)) this.UseToolOnTile(location, player, hoe, tile);
            return true;
        });
    }

    private bool CanTillDirt(Vector2 tile, GameLocation location)
    {
        location.terrainFeatures.TryGetValue(tile, out var tileFeature);
        location.objects.TryGetValue(tile, out var obj);
        return tileFeature is null && obj is null &&
               !location.IsTileOccupiedBy(tile, CollisionMask.All, CollisionMask.Farmers) &&
               location.isTilePassable(tile) &&
               location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is not null;
    }
}