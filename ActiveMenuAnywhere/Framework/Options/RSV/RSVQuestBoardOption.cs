using System.Reflection;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class RSVQuestBoardOption : BaseOption
{
    private readonly IModHelper helper;

    public RSVQuestBoardOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) :
        base(bounds, texture, sourceRect, I18n.Option_RSVQuestBoard())
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var targetDllPath = CommonHelper.GetDllPath(helper, "RidgesideVillage.dll");
        var assembly = Assembly.LoadFrom(targetDllPath);
        var questController = assembly.GetType("RidgesideVillage.Questing.QuestController");
        object[] parameters = { Game1.currentLocation, new[] { "VillageQuestBoard" }, Game1.player, new Point() };
        questController?.GetMethod("OpenQuestBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
    }
}