using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class HarvestGingerHandler : BaseAutomationHandler
{
    public HarvestGingerHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var hoe = ToolHelper.GetTool<Hoe>(this.config.AutoHarvestGinger.FindToolFromInventory);

        if (hoe is null)
        {
            return;
        }

        this.ForEachTile(this.config.AutoHarvestGinger.Range, tile =>
        {
            if (player.Stamina <= this.config.AutoHarvestGinger.StopStamina)
            {
                return false;
            }

            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);

            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                if (hoeDirt.crop.hitWithHoe((int)tile.X, (int)tile.Y, location, hoeDirt))
                {
                    hoeDirt.destroyCrop(true);
                }
            }

            return true;
        });
    }
}
