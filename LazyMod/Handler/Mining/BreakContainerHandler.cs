using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class BreakContainerHandler : BaseAutomationHandler
{
    public BreakContainerHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var weapon = ToolHelper.GetTool<MeleeWeapon>(this.config.AutoBreakContainer.FindToolFromInventory);

        if (weapon is null) return;

        this.ForEachTile(this.config.AutoBreakContainer.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);

            if (obj is BreakableContainer) obj.performToolAction(weapon);

            return true;
        });
    }
}
