using weizinai.StardewValleyMod.Common.Patcher;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Framework.Menu;

namespace weizinai.StardewValleyMod.HelpWanted.Patches;

internal class BillboardPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public BillboardPatcher(ModConfig config)
    {
        BillboardPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<Billboard>(nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
            GetHarmonyMethod(nameof(DrawPrefix))
        );
    }

    private static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        if (!___dailyQuestBoard) return true;
        Game1.activeClickableMenu = new VanillaQuestBoard(config);
        return false;
    }
}