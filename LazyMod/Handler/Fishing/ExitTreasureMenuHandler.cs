using System.Linq;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ExitTreasureMenuHandler : BaseAutomationHandler
{
    public ExitTreasureMenuHandler(ModConfig config) : base(config) { }

    public override bool IsEnable()
    {
        return true;
    }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (Game1.activeClickableMenu is ItemGrabMenu { source: ItemGrabMenu.source_fishingChest } menu)
        {
            var hasItem = menu.ItemsToGrabMenu.actualInventory.OfType<Item>().Any();
            if (!hasItem) menu.exitThisMenu();
        }
    }
}