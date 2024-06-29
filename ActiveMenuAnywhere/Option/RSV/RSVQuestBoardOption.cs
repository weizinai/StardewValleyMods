using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class RSVQuestBoardOption : BaseOption
{
    public RSVQuestBoardOption(Rectangle sourceRect) :
        base(I18n.Option_RSVQuestBoard(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var method = RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.Questing.QuestController", "OpenQuestBoard");
        var parameters = new object[] { Game1.currentLocation, new[] { "VillageQuestBoard" }, Game1.player, new Point() };
        method.Invoke(null, parameters);
    }
}