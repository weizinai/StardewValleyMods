using Common.Patch;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewValley;

namespace SomeMultiplayerFeature.Patches;

internal class UtilityPatcher : BasePatcher
{
    private static IModHelper helper = null!;

    public UtilityPatcher(IModHelper helper)
    {
        UtilityPatcher.helper = helper;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<Utility>(nameof(Utility.TryOpenShopMenu), new[] { typeof(string), typeof(string), typeof(bool) }),
            postfix: GetHarmonyMethod(nameof(TryOpenShopMenuPostfix))
        );
        harmony.Patch(
            RequireMethod<Utility>(nameof(Utility.TryOpenShopMenu),
                new[] { typeof(string), typeof(GameLocation), typeof(Rectangle), typeof(int?), typeof(bool), typeof(bool), typeof(Action<string>) }),
            postfix: GetHarmonyMethod(nameof(TryOpenShopMenuPostfix))
        );
    }

    private static void TryOpenShopMenuPostfix(bool __result, string shopId)
    {
        if (__result)
        {
            var message = new Message(Game1.player.Name, shopId);
            helper.Multiplayer.SendMessage(message, "ShopMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }
    }
}