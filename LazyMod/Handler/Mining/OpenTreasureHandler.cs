using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class OpenTreasureHandler : BaseAutomationHandler
{
    public OpenTreasureHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoOpenTreasure.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId == "(O)-1") obj.checkForAction(player);
            return true;
        });
    }
}