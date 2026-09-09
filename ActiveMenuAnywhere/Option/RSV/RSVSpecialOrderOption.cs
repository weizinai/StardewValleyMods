using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class RSVSpecialOrderOption : BaseOption
{
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
