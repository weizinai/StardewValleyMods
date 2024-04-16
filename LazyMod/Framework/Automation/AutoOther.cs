using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoOther : Automate
{
    private readonly ModConfig config;

    public AutoOther(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;
        
        // 自动清理石头
        if (config.AutoClearStone && (tool is Pickaxe || config.FindPickaxeFromInventory)) AutoClearStone(location, player);
        // 自动清理杂草
        if (config.AutoClearWeeds && (tool is MeleeWeapon || config.FindToolFromInventory)) AutoClearWeeds(location, player);
        // 自动挖掘远古斑点
        if (config.AutoDigArtifactSpots && (tool is Hoe || config.FindHoeFromInventory)) AutoDigArtifactSpots(location, player);
        // 自动收获机器
        if (config.AutoHarvestMachine) AutoHarvestMachine(location, player);
        // 自动触发机器
        if (config.AutoTriggerMachine && item is not null) AutoTriggerMachine(location, player, item);
    }
    
    // 自动清理石头
    private void AutoClearStone(GameLocation location, Farmer player)
    {
        var pickaxe = FindToolFromInventory<Pickaxe>();
        if (pickaxe is null) return;
        
        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearStoneRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is "(O)450" or "(O)343")
            {
                if (StopAutomate(player, config.StopAutoClearStoneStamina, ref hasAddMessage)) break;
                UseToolOnTile(location, player, pickaxe, tile);
            }
        }
    }
    
    // 自动清理杂草
    private void AutoClearWeeds(GameLocation location, Farmer player)
    {
        var scythe = FindToolFromInventory<MeleeWeapon>(true);
        if (scythe is null) return;
        
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearWeedsRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsWeeds())
            {
                obj.performToolAction(scythe);
                location.removeObject(tile, false);
            }
        }
    }
    
    // 自动挖掘远古斑点
    private void AutoDigArtifactSpots(GameLocation location, Farmer player)
    {
        var hoe = FindToolFromInventory<Hoe>();
        if (hoe is null)
            return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoDigArtifactSpotsRange).ToList();
        var hasAddMessage = true;
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId == "(O)590")
            {
                if (StopAutomate(player, config.StopAutoTillDirtStamina, ref hasAddMessage)) break;
                UseToolOnTile(location, player, hoe, tile);
            }
        }
    }

    // 自动收获机器
    private void AutoHarvestMachine(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoHarvestMachineRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.readyForHarvest.Value && obj.heldObject is not null)
                obj.checkForAction(player);
        }
    }
    
    // 自动触发机器
    private void AutoTriggerMachine(GameLocation location, Farmer player, Item item)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoTriggerMachineRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            var machineData = obj?.GetMachineData();
            if (machineData is null) continue;
            if (!MachineDataUtility.HasAdditionalRequirements(SObject.autoLoadFrom ?? player.Items, machineData.AdditionalConsumedItems, out _)) continue;
            obj?.PlaceInMachine(machineData, item, false, player);
        }
    }
}