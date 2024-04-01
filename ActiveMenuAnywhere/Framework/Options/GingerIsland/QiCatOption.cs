using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.Options;

public class QiCatOption : BaseOption
{
    private readonly IModHelper helper;

    public QiCatOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect,I18n.Option_QiCat())
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
        if (isQiWalnutRoomDoorUnlocked)
            helper.Reflection.GetMethod(new GameLocation(), "ShowQiCat").Invoke();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}