using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Farming;

internal class SeedHandler : BaseAutomationHandler
{
    public SeedHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var item = player.CurrentItem;
        if (item is null || item.Category != SObject.SeedsCategory) return;
        
        var grid = this.GetTileGrid(this.Config.AutoSeed.Range);
        
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: null } hoeDirt)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj is not null) continue;

                if (item.Stack <= 0) break;

                if (hoeDirt.plant(item.ItemId, player, false)) player.reduceActiveItemByOne();
            }
        }
    }
}