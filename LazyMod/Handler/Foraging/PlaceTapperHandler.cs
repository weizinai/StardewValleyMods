using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class PlaceTapperHandler : BaseAutomationHandler
{
    public PlaceTapperHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (item is SObject { QualifiedItemId: "(BC)105" or "(BC)264" } tapper)
        {
            this.ForEachTile(this.Config.AutoPlaceTapper.Range, tile =>
            {
                location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
                if (terrainFeature is Tree tree && !tree.tapped.Value) this.PlaceObjectAction(tapper, tile, player, location);
                return true;
            });
        }
    }
}