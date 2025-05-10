using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class QiSpecialOrderOption : BaseOption
{
    public QiSpecialOrderOption()
        : base(I18n.UI_Option_QiSpecialOrder(), TextureManager.Instance.GingerIslandTexture, GetSourceRectangle(0), OptionId.QiSpecialOrder) { }

    public override bool IsEnable()
    {
        return IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
    }
}