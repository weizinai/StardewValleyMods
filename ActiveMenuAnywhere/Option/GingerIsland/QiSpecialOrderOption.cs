using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class QiSpecialOrderOption : BaseOption
{
    public QiSpecialOrderOption(Rectangle sourceRect) :
        base(I18n.UI_Option_QiSpecialOrder(), sourceRect) { }
    
    public override bool IsEnable()
    {
        return IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
    }
}