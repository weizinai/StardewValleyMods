using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class RSVQuestBoardOption : BaseOption
{
    public RSVQuestBoardOption()
        : base(I18n.UI_Option_RSVQuestBoard(),TextureManager.Instance.RSVTexture, GetSourceRectangle(0), OptionId.RSVQuestBoard) { }

    public override void Apply()
    {
        var method = RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.Questing.QuestController", "OpenQuestBoard");
        var parameters = new object[] { Game1.currentLocation, new[] { "VillageQuestBoard" }, Game1.player, new Point() };
        method.Invoke(null, parameters);
    }
}