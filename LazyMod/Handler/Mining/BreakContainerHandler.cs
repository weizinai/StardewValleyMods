using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Mining;

internal class BreakContainerHandler : BaseAutomationHandler
{
    public BreakContainerHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var weapon = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoBreakContainer.FindToolFromInventory);
        if (weapon is null) return;

        var grid = this.GetTileGrid(this.Config.AutoBreakContainer.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is BreakableContainer)
                obj.performToolAction(weapon);
        }
    }
}