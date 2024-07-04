using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class QiCatOption : BaseOption
{
    private readonly IModHelper helper;

    public QiCatOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_QiCat(), sourceRect)
    {
        this.helper = helper;
    }

    public override bool IsEnable()
    {
        return IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
    }

    public override void Apply()
    {
        this.helper.Reflection.GetMethod(new GameLocation(), "ShowQiCat").Invoke();
    }
}