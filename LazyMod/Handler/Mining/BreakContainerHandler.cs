using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class BreakContainerHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var weapon = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoBreakContainer.FindToolFromInventory);
        if (weapon is null) return;

        this.ForEachTile(this.Config.AutoBreakContainer.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is BreakableContainer) obj.performToolAction(weapon);
            return true;
        });
    }
}