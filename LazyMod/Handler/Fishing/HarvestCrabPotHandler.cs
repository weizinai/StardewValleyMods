using StardewValley;
using StardewValley.Objects;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class HarvestCrabPotHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoHarvestCarbPot.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot)
            {
                this.HarvestMachine(player, obj);
            }
            return true;
        });
    }

    protected void HarvestMachine(Farmer player, SObject? machine)
    {
        if (machine == null) return;

        var heldObject = machine.heldObject.Value;
        if (machine.readyForHarvest.Value && heldObject != null)
        {
            if (!player.couldInventoryAcceptThisItem(heldObject)) return;
            machine.checkForAction(player);
        }
    }
}