using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;

internal class Game1Patcher : BasePatcher
{
    private static IModHelper helper = null!;

    public Game1Patcher(IModHelper helper)
    {
        Game1Patcher.helper = helper;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Game1>(nameof(Game1.ShowTelephoneMenu)),
            prefix: this.GetHarmonyMethod(nameof(ShowTelephoneMenuPrefix))
        );
    }

    private static bool ShowTelephoneMenuPrefix()
    {
        if (!ModConfig.Instance.OpenMenuByTelephone) return true;

        Game1.activeClickableMenu = new AMAMenu(ModConfig.Instance.DefaultMenuTabId, helper);

        return false;
    }
}