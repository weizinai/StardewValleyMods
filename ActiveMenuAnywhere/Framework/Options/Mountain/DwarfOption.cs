using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class DwarfOption : BaseOption
{
    public DwarfOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,I18n.Option_Dwarf())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.canUnderstandDwarves)
            Utility.TryOpenShopMenu("Dwarf", "Dwarf");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}