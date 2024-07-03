using StardewValley;
using StardewValley.GameData.Machines;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class TriggerMachineHandler : BaseAutomationHandler
{
    public TriggerMachineHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoTriggerMachine.Range);
        
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null) continue;
            
            var machineData = obj.GetMachineData();
            if (machineData is null) continue;

            if (machineData.AdditionalConsumedItems is not null &&
                !MachineDataUtility.HasAdditionalRequirements(SObject.autoLoadFrom ?? player.Items, machineData.AdditionalConsumedItems, out _))
                continue;

            if (obj.PlaceInMachine(machineData, item, false, player))
            {
                MachineDataUtility.TryGetMachineOutputRule(obj, machineData, MachineOutputTrigger.ItemPlacedInMachine, item, player, location,
                    out _, out var triggerRule, out _, out _);
                if (item.Stack <= triggerRule?.RequiredCount) break;
            }
        }
    }
}