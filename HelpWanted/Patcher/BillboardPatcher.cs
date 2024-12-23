using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework.Menu;

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

    private static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        if (!___dailyQuestBoard) return true;
        Game1.activeClickableMenu = new VanillaQuestBoard();
        return false;
    }
}