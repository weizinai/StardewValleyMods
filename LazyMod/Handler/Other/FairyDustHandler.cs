using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class FairyDustHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item?.QualifiedItemId == "(O)872")
        {
            this.ForEachTile(this.Config.AutoUseFairyDust.Range, tile =>
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj.TryApplyFairyDust()) player.reduceActiveItemByOne();
                return true;
            });
        }
    }
}