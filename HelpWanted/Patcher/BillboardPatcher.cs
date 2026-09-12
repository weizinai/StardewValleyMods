using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.HelpWanted.UI;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class BillboardPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<Billboard>(harmony, nameof(Billboard.draw), PatchKind.Prefix, nameof(DrawPrefix), new[] { typeof(SpriteBatch) });
    }

    // 将原版任务菜单替换为自定义菜单
    private static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        if (!___dailyQuestBoard)
        {
            return true;
        }

        Logger<ModEntry>.Trace("Detected activation of the vanilla daily quest menu. It has been replaced with the custom menu.");

        Game1.activeClickableMenu.exitThisMenuNoSound();
        Game1.activeClickableMenu = new VanillaQuestBoard();

        return false;
    }
}
