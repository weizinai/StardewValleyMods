using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DwarfOption : BaseOption
{
    public DwarfOption(Rectangle sourceRect) :
        base(I18n.UI_Option_Dwarf(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.canUnderstandDwarves;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Dwarf", "Dwarf");
    }
}