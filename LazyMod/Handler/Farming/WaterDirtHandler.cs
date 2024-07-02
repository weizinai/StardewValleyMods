using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Farming;

internal class WaterDirtHandler : BaseAutomationHandler
{
    public WaterDirtHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var wateringCan = ToolHelper.GetTool<WateringCan>(this.Config.AutoWaterDirt.FindToolFromInventory);
        if (wateringCan is null) return;
        
        var grid = this.GetTileGrid(this.Config.AutoWaterDirt.Range);
        
        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoWaterDirt.StopStamina || wateringCan.WaterLeft <= 0) break;
            
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            if (tileFeature is HoeDirt hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                this.UseToolOnTile(location, player, wateringCan, tile);
                if (wateringCan.WaterLeft > 0 && player.ShouldHandleAnimationSound())
                {
                    player.playNearbySoundLocal("wateringCan");
                }
            }
        }
    }
}