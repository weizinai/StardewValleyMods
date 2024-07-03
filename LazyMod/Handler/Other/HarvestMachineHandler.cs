using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class HarvestMachineHandler : BaseAutomationHandler
{
    public HarvestMachineHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoHarvestMachine.Range);
        
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot) continue;
            this.HarvestMachine(player, obj);
        }
    }

    private void HarvestMachine(Farmer player, SObject? machine)
    {
        if (machine is null) return;

        var heldObject = machine.heldObject.Value;
        if (machine.readyForHarvest.Value && heldObject is not null)
        {
            if (player.freeSpotsInInventory() == 0 && !player.Items.ContainsId(heldObject.ItemId)) return;
            machine.checkForAction(player);
        }
    }
}