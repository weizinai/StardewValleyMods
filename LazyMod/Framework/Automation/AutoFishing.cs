using LazyMod.Framework.Config;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

internal class AutoFishing : Automate
{
    public AutoFishing(ModConfig config): base(config)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        TileCache.Clear();
        // 自动放置蟹笼
        if (Config.AutoPlaceCarbPot && item is SObject { QualifiedItemId: "(O)710" } crabPot) AutoPlaceCrabPot(location, player, crabPot);
        // 自动添加蟹笼鱼饵
        if (Config.AutoAddBaitForCarbPot && item is SObject { Category: SObject.baitCategory } bait) AutoAddBaitForCrabPot(location, player, bait);
        // 自动收获蟹笼
        if (Config.AutoHarvestCarbPot) AutoHarvestCarbPot(location, player);
        TileCache.Clear();
    }

    public void AutoMenuFunction()
    {
        if (Game1.activeClickableMenu is ItemGrabMenu { source: ItemGrabMenu.source_fishingChest } menu)
        {
            if (Config.AutoGrabTreasureItem) AutoGrabTreasureItem(menu);
            if (Config.AutoExitTreasureMenu) AutoExitTreasureMenu(menu);
        }
    }

    // 自动抓取宝箱物品
    private void AutoGrabTreasureItem(ItemGrabMenu menu)
    {
        var items = menu.ItemsToGrabMenu.actualInventory;
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i] is null) continue;
            if (!CanAddItemToInventory(items[i])) break;

            var center = menu.ItemsToGrabMenu.inventory[i].bounds.Center;
            menu.receiveLeftClick(center.X, center.Y);
        }
    }

    // 自动退出宝箱菜单
    private void AutoExitTreasureMenu(ItemGrabMenu menu)
    {
        var hasItem = menu.ItemsToGrabMenu.actualInventory.OfType<Item>().Any();
        if (!hasItem) menu.exitThisMenu();
    }

    // 自动放置蟹笼
    private void AutoPlaceCrabPot(GameLocation location, Farmer player, SObject crabPot)
    {
        var grid = GetTileGrid(player, Config.AutoPlaceCarbPotRange);
        foreach (var _ in grid.Select(tile => GetTilePixelPosition(tile))
                     .Where(tilePixelPosition => crabPot.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)))
        {
            player.reduceActiveItemByOne();
        }
    }

    // 自动添加蟹笼鱼饵
    private void AutoAddBaitForCrabPot(GameLocation location, Farmer player, SObject bait)
    {
        var grid = GetTileGrid(player, Config.AutoAddBaitForCarbPotRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not CrabPot crabPot || crabPot.bait.Value is not null) continue;
            if (obj.performObjectDropInAction(bait, false, player)) ConsumeItem(player, bait);
        }
    }

    // 自动收获蟹笼
    private void AutoHarvestCarbPot(GameLocation location, Farmer player)
    {
        var grid = GetTileGrid(player, Config.AutoHarvestCarbPotRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot) HarvestMachine(player, obj);
        }
    }
}