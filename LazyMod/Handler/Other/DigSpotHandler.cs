using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Other;

internal class DigSpotHandler : BaseAutomationHandler
{
    public DigSpotHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var hoe = ToolHelper.GetTool<Hoe>(this.Config.AutoDigSpots.FindToolFromInventory);
        if (hoe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoDigSpots.Range);
        
        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoDigSpots.StopStamina) return;
            
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is not ("(O)590" or "(O)SeedSpot")) continue;
            this.UseToolOnTile(location, player, hoe, tile);
        }
    }
}