using StardewValley;
using StardewValley.Buffs;
using StardewValley.Characters;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoOther : Automate
{
    private const string UniqueBuffId = "weizinai.LazyMod";
    private readonly ModConfig config;

    public AutoOther(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;
        
        // 增加磁力范围
        MagneticRadiusIncrease(player);
        TileCache.Clear();
        // 自动清理杂草
        if (config.AutoClearWeeds && (tool is MeleeWeapon || config.FindToolFromInventory)) AutoClearWeeds(location, player);
        // 自动挖掘远古斑点
        if (config.AutoDigSpots && (tool is Hoe || config.FindHoeFromInventory)) AutoDigArtifactSpots(location, player);
        // 自动收获机器
        if (config.AutoHarvestMachine) AutoHarvestMachine(location, player);
        // 自动触发机器
        if (config.AutoTriggerMachine && item is not null) AutoTriggerMachine(location, player, item);
        // 自动翻垃圾桶
        if (config.AutoGarbageCan) AutoGarbageCan(location, player);
        TileCache.Clear();
    }

    // 增加磁力范围
    private void MagneticRadiusIncrease(Farmer player)
    {
        if (config.MagneticRadiusIncrease == 0)
        {
            player.buffs.Remove(UniqueBuffId);
            return;
        }

        player.buffs.AppliedBuffs.TryGetValue(UniqueBuffId, out var buff);
        if (buff is null || buff.millisecondsDuration <= 5000 || Math.Abs(buff.effects.MagneticRadius.Value - config.MagneticRadiusIncrease) > 0.1f)
        {
            buff = new Buff(
                id: UniqueBuffId,
                source: "Lazy Mod",
                duration: 60000,
                effects: new BuffEffects
                {
                    MagneticRadius = { Value = config.MagneticRadiusIncrease }
                });
            player.applyBuff(buff);
        }
    }

    // 自动清理杂草
    private void AutoClearWeeds(GameLocation location, Farmer player)
    {
        var scythe = FindToolFromInventory<MeleeWeapon>(true);
        if (scythe is null) return;

        var grid = GetTileGrid(player, config.AutoClearWeedsRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsWeeds() && obj.QualifiedItemId is not ("(O)319" or "(O)320" or "(O)321"))
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

        var grid = GetTileGrid(player, config.AutoDigSpotsRange);
        var hasAddMessage = true;
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is not ("(O)590" or "(O)SeedSpot")) continue;
            if (StopAutomate(player, config.StopAutoTillDirtStamina, ref hasAddMessage)) break;
            UseToolOnTile(location, player, hoe, tile);
        }
    }

    // 自动收获机器
    private void AutoHarvestMachine(GameLocation location, Farmer player)
    {
        var grid = GetTileGrid(player, config.AutoHarvestMachineRange);
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
        var grid = GetTileGrid(player, config.AutoTriggerMachineRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            var machineData = obj?.GetMachineData();
            if (machineData is null) continue;
            if (!MachineDataUtility.HasAdditionalRequirements(SObject.autoLoadFrom ?? player.Items, machineData.AdditionalConsumedItems, out _)) continue;
            obj?.PlaceInMachine(machineData, item, false, player);
        }
    }

    // 自动翻垃圾桶
    private void AutoGarbageCan(GameLocation location, Farmer player)
    {
        if (CheckNPCNearTile(location, player) && config.StopAutoGarbageCanNearVillager) return;
        var grid = GetTileGrid(player, config.AutoCollectCoalRange);
        foreach (var tile in grid)
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 78)
                CheckTileAction(location, player, tile);
        }
    }

    /// <summary>
    /// 检测周围是否有NPC
    /// </summary>
    /// <returns>如果有,则返回true,否则返回false</returns>
    private bool CheckNPCNearTile(GameLocation location, Farmer player)
    {
        var tile = player.Tile;
        var npcs = Utility.GetNpcsWithinDistance(tile, 7, location).ToList();
        if (!npcs.Any()) return false;
        var horse = npcs.FirstOrDefault(npc => npc is Horse);
        return horse is null || npcs.Count != 1;
    }
}