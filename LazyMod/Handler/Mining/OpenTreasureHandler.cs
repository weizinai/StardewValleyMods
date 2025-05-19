using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class OpenTreasureHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoOpenTreasure.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId == "(O)-1")
            {
                obj.checkForAction(player);
            }
            return true;
        });
    }
}