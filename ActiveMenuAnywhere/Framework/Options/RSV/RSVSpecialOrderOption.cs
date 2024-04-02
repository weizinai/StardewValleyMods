using System.Reflection;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class RSVSpecialOrderOption : BaseOption
{
    private readonly IModHelper helper;

    public RSVSpecialOrderOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) :
        base(bounds, texture, sourceRect, I18n.Option_RSVSpecialOrder())
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.eventsSeen.Contains("75160207"))
        {
            var targetDllPath = CommonHelper.GetDllPath(helper, "RidgesideVillage.dll");
            var assembly = Assembly.LoadFrom(targetDllPath);
            var questController = assembly.GetType("RidgesideVillage.Questing.QuestController");
            object[] parameters = { Game1.currentLocation, new[] { "RSVTownSO" }, Game1.player, new Point() };
            questController?.GetMethod("OpenSOBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }
}