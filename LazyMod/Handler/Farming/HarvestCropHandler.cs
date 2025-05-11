using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class HarvestCropHandler : BaseAutomationHandler
{
    public HarvestCropHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoHarvestCrop.Range, tile =>
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;

                // 自动收获花逻辑
                if (!this.Config.AutoHarvestFlower && ItemRegistry.GetData(crop.indexOfHarvest.Value)?.Category == SObject.flowersCategory)
                    return true;

                if (crop.harvest((int)tile.X, (int)tile.Y, hoeDirt))
                {
                    hoeDirt.destroyCrop(true);
                    // 姜岛金核桃逻辑
                    if (location is IslandLocation && Game1.random.NextDouble() < 0.05)
                        player.team.RequestLimitedNutDrops("IslandFarming", location, (int)tile.X * 64, (int)tile.Y * 64, 5);
                }
            }

            return true;
        });
    }
}