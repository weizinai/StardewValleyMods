using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class SpecialOrderMenu: BaseActiveMenu
{
    private IModHelper helper;
    
    public SpecialOrderMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var isShowingSpecialOrdersBoard = helper.Reflection.GetField<bool>(new Town(), "isShowingSpecialOrdersBoard").GetValue();
        if (isShowingSpecialOrdersBoard)
            Game1.activeClickableMenu = new SpecialOrdersBoard();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}