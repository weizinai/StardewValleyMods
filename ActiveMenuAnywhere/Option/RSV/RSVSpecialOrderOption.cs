using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class RSVSpecialOrderOption : BaseOption
{
    public RSVSpecialOrderOption(Rectangle sourceRect) :
        base(I18n.UI_Option_RSVSpecialOrder(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("75160207");
    }

    public override void Apply()
    {
        var method = RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.Questing.QuestController", "OpenSOBoard");
        var parameters = new object[] { Game1.currentLocation, new[] { "RSVTownSO" }, Game1.player, Point.Zero };
        method.Invoke(null, parameters);
    }
}