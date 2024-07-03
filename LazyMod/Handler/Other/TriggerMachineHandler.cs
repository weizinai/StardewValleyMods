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
        this.ForEachTile(this.Config.AutoTriggerMachine.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);

            var machineData = obj?.GetMachineData();
            if (machineData == null) return true;

            if (machineData.AdditionalConsumedItems != null &&
                !MachineDataUtility.HasAdditionalRequirements(SObject.autoLoadFrom ?? player.Items, machineData.AdditionalConsumedItems, out _))
                return true;

            if (obj?.PlaceInMachine(machineData, item, false, player) == true)
            {
                MachineDataUtility.TryGetMachineOutputRule(obj, machineData, MachineOutputTrigger.ItemPlacedInMachine, item, player, location,
                    out _, out var triggerRule, out _, out _);
                if (item.Stack <= triggerRule?.RequiredCount) return false;
            }

            return true;
        });
    }
}