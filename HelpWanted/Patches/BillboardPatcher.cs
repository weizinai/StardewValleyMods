﻿using Common.Patcher;
using HarmonyLib;
using HelpWanted.Framework;
using HelpWanted.Framework.Menu;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace HelpWanted.Patches;

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