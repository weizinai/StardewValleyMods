using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Config;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.UI;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<Game1>(harmony, nameof(Game1.ShowTelephoneMenu), PatchKind.Prefix, nameof(ShowTelephoneMenuPrefix));
    }

    private static bool ShowTelephoneMenuPrefix()
    {
        if (!ModConfig.Instance.OpenMenuByTelephone)
        {
            return true;
        }

        MenuLauncher.Open(ModConfig.Instance.DefaultMenuTabId);

        return false;
    }
}
