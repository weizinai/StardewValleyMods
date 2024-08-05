using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class ForgeOption : BaseOption
{
    public ForgeOption() : base(I18n.UI_Option_Forge(), GetSourceRectangle(6)) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("willyHours");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new ForgeMenu();
    }
}