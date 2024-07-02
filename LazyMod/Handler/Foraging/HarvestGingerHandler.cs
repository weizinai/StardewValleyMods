using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Foraging;

internal class HarvestGingerHandler : BaseAutomationHandler
{
    public HarvestGingerHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var hoe = ToolHelper.GetTool<Hoe>(this.Config.AutoHarvestGinger.FindToolFromInventory);
        if (hoe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoHarvestGinger.Range);
        
        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoHarvestGinger.StopStamina) return;
            
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                if (hoeDirt.crop.hitWithHoe((int)tile.X, (int)tile.Y, location, hoeDirt)) hoeDirt.destroyCrop(true);
            }
        }
    }
}