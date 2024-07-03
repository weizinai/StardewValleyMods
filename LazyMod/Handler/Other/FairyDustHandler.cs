using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class FairyDustHandler : BaseAutomationHandler
{
    public FairyDustHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (item.QualifiedItemId == "(O)872")
        {
            var grid = this.GetTileGrid(this.Config.AutoUseFairyDust.Range);

            foreach (var tile in grid)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj.TryApplyFairyDust()) player.reduceActiveItemByOne();
            }
        }
    }
}