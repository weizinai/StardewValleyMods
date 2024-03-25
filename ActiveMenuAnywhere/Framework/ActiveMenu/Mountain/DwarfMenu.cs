using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class DwarfMenu : BaseActiveMenu
{
    public DwarfMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.canUnderstandDwarves)
            Utility.TryOpenShopMenu("Dwarf", "Dwarf");
        else
            Game1.drawObjectDialogue("不好意思，你还不会矮人语");
    }
}