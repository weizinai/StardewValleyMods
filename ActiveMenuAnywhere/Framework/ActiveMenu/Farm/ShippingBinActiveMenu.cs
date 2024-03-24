using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Farm;

public class ShippingBinActiveMenu: BaseActiveMenu
{
    private IModHelper helper;
    
    public ShippingBinActiveMenu(IModHelper helper, Rectangle bounds, Texture2D texture, Rectangle sourceRect):base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        helper.Reflection.GetMethod(new ShippingBin(), "leftClicked").Invoke(new Vector2(1,1), Game1.player);
    }
}