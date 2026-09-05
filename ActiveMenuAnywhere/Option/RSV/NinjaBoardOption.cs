using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class NinjaBoardOption : BaseOption
{
    public NinjaBoardOption()
        : base(I18n.UI_Option_NinjaBoard(), TextureManager.Instance.RSVTexture, GetSourceRectangle(10), OptionId.NinjaBoard) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160254");
    }

    public override void Apply()
    {
        var method = RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.Questing.QuestController", "OpenQuestBoard");
        var parameters = new object[] { Game1.currentLocation, new[] { "RSVNinjaBoard" }, Game1.player, new Point() };
        method.Invoke(null, parameters);
    }
}