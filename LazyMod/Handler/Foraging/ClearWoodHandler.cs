using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Foraging;

internal class ClearWoodHandler : BaseAutomationHandler
{
    public ClearWoodHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var axe = ToolHelper.GetTool<Axe>(this.Config.AutoClearWood.FindToolFromInventory);
        if (axe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoClearWood.Range);
        
        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoClearWood.StopStamina) return;

            if (this.Config.ClearTwig)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj is not null && obj.IsTwig())
                {
                    this.UseToolOnTile(location, player, axe, tile);
                }
            }

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(this.GetTileBoundingBox(tile))) continue;

                var clear = false;
                var requiredUpgradeLevel = Tool.stone;

                if (this.Config.ClearStump && clump.parentSheetIndex.Value == ResourceClump.stumpIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.copper;
                }

                if (this.Config.ClearHollowLog && clump.parentSheetIndex.Value == ResourceClump.hollowLogIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.steel;
                }

                if (clear && axe.UpgradeLevel >= requiredUpgradeLevel)
                {
                    this.UseToolOnTile(location, player, axe, tile);
                    break;
                }
            }
        }
    }
}