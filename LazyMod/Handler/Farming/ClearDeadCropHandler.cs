using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearDeadCropHandler : BaseAutomationHandler
{
    public ClearDeadCropHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var scythe = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoClearDeadCrop.FindToolFromInventory);
        if (scythe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoHarvestCrop.Range);
        
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                if (crop.dead.Value) hoeDirt.performToolAction(scythe, 0, tile);
            }
        }
    }
}