using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class QiSpecialOrderOption : BaseOption
{
    public QiSpecialOrderOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,I18n.Option_QiSpecialOrder())
    {
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
        if (isQiWalnutRoomDoorUnlocked)
            Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}