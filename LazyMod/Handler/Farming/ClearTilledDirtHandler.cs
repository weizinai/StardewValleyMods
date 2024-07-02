using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearTilledDirtHandler : BaseAutomationHandler
{
    public ClearTilledDirtHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var pickaxe = ToolHelper.GetTool<Pickaxe>(this.Config.AutoClearTilledDirt.FindToolFromInventory);
        if (pickaxe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoClearTilledDirt.Range);
        
        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoClearTilledDirt.StopStamina) break;
            
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            if (tileFeature is HoeDirt { crop: null } hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                this.UseToolOnTile(location, player, pickaxe, tile);
            }
        }
        
    }
}