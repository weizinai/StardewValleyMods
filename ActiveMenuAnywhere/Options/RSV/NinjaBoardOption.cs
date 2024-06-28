using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class NinjaBoardOption : BaseOption
{
    public NinjaBoardOption(Rectangle sourceRect) :
        base(I18n.Option_NinjaBoard(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        // if (Game1.player.eventsSeen.Contains("75160254"))
        // {
        //     var questController = RSVIntegration.GetType("RidgesideVillage.Questing.QuestController");
        //     object[] parameters = { Game1.currentLocation, new[] { "RSVNinjaBoard" }, Game1.player, new Point() };
        //     questController?.GetMethod("OpenQuestBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
        // }
        // else
        // {
        //     Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        // }
    }
}