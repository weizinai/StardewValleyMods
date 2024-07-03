using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class PlaceVinegarHandler : BaseAutomationHandler
{
    public PlaceVinegarHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (item is SObject { QualifiedItemId: "(O)419" } vinegar)
        {
            this.ForEachTile(this.Config.AutoPlaceVinegar.Range, tile =>
            {
                location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
                if (terrainFeature is Tree tree && !tree.stopGrowingMoss.Value) this.PlaceObjectAction(vinegar, tile, player, location);
                return true;
            });
        }
    }
}