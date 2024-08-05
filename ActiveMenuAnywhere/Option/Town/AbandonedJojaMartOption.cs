using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class AbandonedJojaMartOption : BaseOption
{
    public AbandonedJojaMartOption()
        : base(I18n.UI_Option_AbandonedJojaMart(), TextureManager.Instance.TownTexture, GetSourceRectangle(15), OptionId.AbandonedJojaMart) { }

    public override bool IsEnable()
    {
        return Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible");
    }

    public override void Apply()
    {
        var abandonedJojaMart = Game1.RequireLocation<AbandonedJojaMart>("AbandonedJojaMart");
        abandonedJojaMart.checkBundle();
    }
}