using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class DwarfOption : BaseOption
{
    public DwarfOption()
        : base(I18n.UI_Option_Dwarf(), TextureManager.Instance.MountainTexture, GetSourceRectangle(1), OptionId.Dwarf) { }

    public override bool IsEnable()
    {
        return Game1.player.canUnderstandDwarves;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Dwarf", "Dwarf");
    }
}