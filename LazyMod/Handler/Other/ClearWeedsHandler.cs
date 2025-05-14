using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Helper;
using weizinai.StardewValleyMod.PiCore.Extension;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class ClearWeedsHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var scythe = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoClearWeeds.FindToolFromInventory);
        if (scythe is null) return;

        this.ForEachTile(this.Config.AutoClearWeeds.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj != null && obj.IsWeeds() && !obj.IsIceCrystal())
            {
                obj.performToolAction(scythe);
                location.removeObject(tile, false);
                return true;
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

            return true;
        });
    }
}