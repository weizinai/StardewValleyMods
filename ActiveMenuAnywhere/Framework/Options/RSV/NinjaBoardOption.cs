using System.Reflection;
using Common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class NinjaBoardOption : BaseOption
{
    private readonly IModHelper helper;

    public NinjaBoardOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_NinjaBoard(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160254"))
        {
            var targetDllPath = CommonHelper.GetDllPath(helper, "RidgesideVillage.dll");
            var assembly = Assembly.LoadFrom(targetDllPath);
            var questController = assembly.GetType("RidgesideVillage.Questing.QuestController");
            object[] parameters = { Game1.currentLocation, new[] { "RSVNinjaBoard" }, Game1.player, new Point() };
            questController?.GetMethod("OpenQuestBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }
}