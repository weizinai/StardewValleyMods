using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace ActiveMenuAnywhere.Framework.Options;

public class ShippingBinOption : BaseOption
{
    private readonly IModHelper helper;

    public ShippingBinOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect,I18n.Option_ShippingBin())
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var itemGrabMenu = new ItemGrabMenu(null, true, false, Utility.highlightShippableObjects,
            ShipItem, "", null, true, true, false,
            true, false, 0, null, -1, this);
        itemGrabMenu.initializeUpperRightCloseButton();
        itemGrabMenu.setBackgroundTransparency(false);
        itemGrabMenu.setDestroyItemOnClick(true);
        itemGrabMenu.initializeShippingBin();
        Game1.activeClickableMenu = itemGrabMenu;
        if (Game1.player.IsLocalPlayer) Game1.playSound("shweip");
        if (Game1.player.FacingDirection == 1) Game1.player.Halt();
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
                helper.Reflection.GetMethod(new ShippingBin(), "showShipment").Invoke(obj, false);
            if (farm != null)
                farm.lastItemShipped = i;
            if (Game1.player.ActiveObject == null)
            {
                Game1.player.showNotCarrying();
                Game1.player.Halt();
            }
        }
    }
}