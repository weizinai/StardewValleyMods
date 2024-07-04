using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class NinjaBoardOption : BaseOption
{
    public NinjaBoardOption(Rectangle sourceRect) :
        base(I18n.Option_NinjaBoard(), sourceRect) { }

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