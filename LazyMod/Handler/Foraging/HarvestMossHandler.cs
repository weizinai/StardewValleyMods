using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class HarvestMossHandler : BaseAutomationHandler
{
    public HarvestMossHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var scythe = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoHarvestMoss.FindToolFromInventory);
        if (scythe is null) return;

        this.ForEachTile(this.Config.AutoHarvestMoss.Range, tile =>
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasMoss.Value) tree.performToolAction(scythe, 0, tile);
            return true;
        });
    }
}