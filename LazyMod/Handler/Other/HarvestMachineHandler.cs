using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class HarvestMachineHandler : BaseAutomationHandler
{
    public HarvestMachineHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoHarvestMachine.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not CrabPot) this.HarvestMachine(player, obj);
            return true;
        });
    }

    private void HarvestMachine(Farmer player, SObject? machine)
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