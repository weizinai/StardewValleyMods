using StardewValley;
using StardewValley.Buffs;
using StardewValley.Characters;
using StardewValley.Objects;
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

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 增加磁力范围
        MagneticRadiusIncrease(player);
        TileCache.Clear();
        // 自动清理杂草
        if (config.AutoClearWeeds && (tool is MeleeWeapon || config.FindToolForClearWeeds)) AutoClearWeeds(location, player);
        // 自动挖掘斑点
        if (config.AutoDigSpots && (tool is Hoe || config.FindHoeFromInventory)) AutoDigSpots(location, player);
        // 自动收获机器
        if (config.AutoHarvestMachine) AutoHarvestMachine(location, player);
        // 自动触发机器
        if (config.AutoTriggerMachine && item is not null) AutoTriggerMachine(location, player, item);
        // 自动翻垃圾桶
        if (config.AutoGarbageCan) AutoGarbageCan(location, player);
        // 自动放置地板
        if (config.AutoPlaceFloor && (item is SObject floor) && floor.IsFloorPathItem()) AutoPlaceFloor(location, player, floor);
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

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(GetTileBoundingBox(tile))) continue;

                if (config.ClearLargeWeeds && clump.parentSheetIndex.Value is 44 or 46)
                {
                    UseToolOnTile(location, player, scythe, tile);
                    if (clump.performToolAction(scythe, 1, tile))
                    {
                        location.resourceClumps.Remove(clump);
                        break;
                    }
                }
            }
        }
    }

    // 自动挖掘斑点
    private void AutoDigSpots(GameLocation location, Farmer player)
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
            if (StopAutomate(player, config.StopTillDirtStamina, ref hasAddMessage)) break;
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
            if (obj is CrabPot) continue;
            HarvestMachine(player, obj);
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
        if (CheckNPCNearTile(location, player) && config.StopGarbageCanNearVillager) return;
        var grid = GetTileGrid(player, config.AutoGarbageCanRange);
        foreach (var tile in grid)
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 78)
                CheckTileAction(location, player, tile);
        }
    }
    
    // 自动放置地板
    private void AutoPlaceFloor(GameLocation location, Farmer player, SObject floor)
    {
        var grid = GetTileGrid(player, config.AutoPlaceFloorRange);
        foreach (var tile in grid)
        {
            var tilePixelPosition = GetTilePixelPosition(tile);
            if (floor.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) ConsumeItem(player, floor);
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