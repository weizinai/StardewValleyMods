using Microsoft.Xna.Framework;
using StardewModdingAPI;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class RSVQuestBoardOption : BaseOption
{
    private readonly IModHelper helper;

    public RSVQuestBoardOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_RSVQuestBoard(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        // var questController = RSVIntegration.GetType("RidgesideVillage.Questing.QuestController");
        // object[] parameters = { Game1.currentLocation, new[] { "VillageQuestBoard" }, Game1.player, new Point() };
        // questController?.GetMethod("OpenQuestBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
    }
}