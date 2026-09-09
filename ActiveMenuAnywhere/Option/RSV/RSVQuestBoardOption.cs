using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class RSVQuestBoardOption : BaseOption
{
    public override void Apply()
    {
        var method = RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.Questing.QuestController", "OpenQuestBoard");
        var parameters = new object[] { Game1.currentLocation, new[] { "VillageQuestBoard" }, Game1.player, new Point() };
        method.Invoke(null, parameters);
    }
}
