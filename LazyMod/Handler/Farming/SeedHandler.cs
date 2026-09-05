using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class SeedHandler : BaseAutomationHandler
{
    public SeedHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item?.Category == SObject.SeedsCategory)
        {
            this.ForEachTile(this.Config.AutoSeed.Range, tile =>
            {
                location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
                if (terrainFeature is HoeDirt { crop: null } hoeDirt)
                {
                    if (item.Stack <= 0) return false;

                    location.objects.TryGetValue(tile, out var obj);
                    if (obj is not null) return true;

                    if (hoeDirt.plant(item.ItemId, player, false)) player.reduceActiveItemByOne();
                }

                return true;
            });
        }
    }
}