using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Menu;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class BillboardPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Billboard>(nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
            prefix: this.GetHarmonyMethod(nameof(DrawPrefix))
        );
    }

    // 将原版任务菜单替换为自定义菜单
    private static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        if (!___dailyQuestBoard) return true;

        Logger.Trace("Detected activation of the vanilla daily quest menu. It has been replaced with the custom menu.");

        Game1.activeClickableMenu.exitThisMenuNoSound();
        Game1.activeClickableMenu = new VanillaQuestBoard();

        return false;
    }
}