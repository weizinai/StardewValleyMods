using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class AbandonedJojaMartOption : BaseOption
{
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
