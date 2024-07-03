using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class HarvestCrabPotHandler : BaseAutomationHandler
{
    public HarvestCrabPotHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoHarvestCarbPot.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot) this.HarvestMachine(player, obj);
        }
    }

    protected void HarvestMachine(Farmer player, SObject? machine)
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