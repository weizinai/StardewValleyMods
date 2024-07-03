using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearWeedsHandler : BaseAutomationHandler
{
    public ClearWeedsHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var scythe = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoClearWeeds.FindToolFromInventory);
        if (scythe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoClearWeeds.Range);
        
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsWeeds() && obj.QualifiedItemId is not ("(O)319" or "(O)320" or "(O)321"))
            {
                obj.performToolAction(scythe);
                location.removeObject(tile, false);
            }

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(this.GetTileBoundingBox(tile))) continue;

                if (this.Config.ClearLargeWeeds && clump.parentSheetIndex.Value is 44 or 46)
                {
                    scythe.swingTicker++;
                    if (clump.performToolAction(scythe, 1, tile))
                    {
                        location.resourceClumps.Remove(clump);
                        break;
                    }
                }
            }
        }
    }
}