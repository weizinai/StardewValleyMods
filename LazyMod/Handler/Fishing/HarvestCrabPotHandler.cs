using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class HarvestCrabPotHandler : BaseAutomationHandler
{
    public HarvestCrabPotHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoHarvestCarbPot.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot) this.HarvestMachine(player, obj);
            return true;
        });
    }

    protected void HarvestMachine(Farmer player, SObject? machine)
    {
        if (machine is null) return;

        var heldObject = machine.heldObject.Value;
        if (machine.readyForHarvest.Value && heldObject is not null)
        {
            if (!player.couldInventoryAcceptThisItem(heldObject)) return;
            machine.checkForAction(player);
        }
    }
}