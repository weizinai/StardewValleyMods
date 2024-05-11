using Common.Patch;
using HarmonyLib;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature.Patches;

// ReSharper disable once InconsistentNaming
internal class IClickableMenuPatcher : BasePatcher
{
    private static IModHelper helper = null!;

    public IClickableMenuPatcher(IModHelper helper)
    {
        IClickableMenuPatcher.helper = helper;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<IClickableMenu>(nameof(IClickableMenu.exitThisMenu), new[] { typeof(bool) }),
            postfix: GetHarmonyMethod(nameof(ExitThisMenuPostfix))
        );
    }

    private static void ExitThisMenuPostfix(IClickableMenu __instance)
    {
        if (__instance is ShopMenu shopMenu)
        {
            var message = new Message(Game1.player.Name, shopMenu.ShopId, true);
            helper.Multiplayer.SendMessage(message, "ShopMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }
    }
}