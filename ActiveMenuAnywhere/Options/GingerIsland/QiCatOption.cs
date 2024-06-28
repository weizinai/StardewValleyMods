using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class QiCatOption : BaseOption
{
    private readonly IModHelper helper;

    public QiCatOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_QiCat(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
        if (isQiWalnutRoomDoorUnlocked)
            this.helper.Reflection.GetMethod(new GameLocation(), "ShowQiCat").Invoke();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}