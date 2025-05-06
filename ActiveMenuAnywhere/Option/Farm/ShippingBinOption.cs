﻿using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using Object = StardewValley.Object;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class ShippingBinOption : BaseOption
{
    private readonly IModHelper helper;

    public ShippingBinOption(IModHelper helper)
        : base(I18n.UI_Option_ShippingBin(), TextureManager.Instance.FarmTexture, GetSourceRectangle(1), OptionId.ShippingBin)
    {
        this.helper = helper;
    }

    public override void Apply()
    {
        var itemGrabMenu = new ItemGrabMenu(
            null,
            true,
            false,
            Utility.highlightShippableObjects,
            this.ShipItem,
            "",
            null,
            true,
            true,
            false,
            true,
            false,
            0,
            null,
            -1,
            this
        );
        itemGrabMenu.initializeUpperRightCloseButton();
        itemGrabMenu.setBackgroundTransparency(false);
        itemGrabMenu.setDestroyItemOnClick(true);
        itemGrabMenu.initializeShippingBin();
        Game1.activeClickableMenu = itemGrabMenu;
        Game1.player.showCarrying();
    }

    private void ShipItem(Item? i, Farmer who)
    {
        var farm = Game1.RequireLocation<Farm>("farm");
        if (i != null)
        {
            who.removeItemFromInventory(i);
            farm?.getShippingBin(who).Add(i);
            
            if (i is Object obj && farm != null)
            {
                this.helper.Reflection.GetMethod(new ShippingBin(), "showShipment").Invoke(obj, false);
            }
            
            if (farm != null)
            {
                farm.lastItemShipped = i;
            }
            
            if (Game1.player.ActiveObject == null)
            {
                Game1.player.showNotCarrying();
                Game1.player.Halt();
            }
        }
    }
}