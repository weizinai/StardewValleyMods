using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class OpenTreasureHandler : BaseAutomationHandler
{
    public OpenTreasureHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoBreakContainer.Range);
        
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null || obj.QualifiedItemId != "(O)-1") continue;
            obj.checkForAction(player);
        }
    }
}