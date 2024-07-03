using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class HarvestMossHandler : BaseAutomationHandler
{
    public HarvestMossHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var scythe = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoHarvestMoss.FindToolFromInventory);
        if (scythe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoHarvestMoss.Range);
        
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasMoss.Value)
                tree.performToolAction(scythe, 0, tile);
        }
    }
}