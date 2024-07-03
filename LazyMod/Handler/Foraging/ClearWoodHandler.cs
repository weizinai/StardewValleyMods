using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearWoodHandler : BaseAutomationHandler
{
    public ClearWoodHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var axe = ToolHelper.GetTool<Axe>(this.Config.AutoClearWood.FindToolFromInventory);
        if (axe is null) return;

        this.ForEachTile(this.Config.AutoClearWood.Range, tile =>
        {
            if (player.Stamina <= this.Config.AutoClearWood.StopStamina) return false;

            if (this.Config.ClearTwig)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj?.IsTwig() == true)
                {
                    this.UseToolOnTile(location, player, axe, tile);
                    return true;
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
            
            return true;
        }); 
    }
}