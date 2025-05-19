using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class GrabTreasureItemHandler : BaseAutomationHandler
{
    public override bool IsEnable()
    {
        return true;
    }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (Game1.activeClickableMenu is ItemGrabMenu { source: ItemGrabMenu.source_fishingChest } menu)
        {
            var items = menu.ItemsToGrabMenu.actualInventory;
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] == null) continue;
                if (!player.couldInventoryAcceptThisItem(items[i])) break;

                var center = menu.ItemsToGrabMenu.inventory[i].bounds.Center;
                menu.receiveLeftClick(center.X, center.Y);
            }
        }
    }
}