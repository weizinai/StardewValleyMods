using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.SaveModInfo.Handler;
using static StardewValley.Menus.LoadGameMenu;

namespace weizinai.StardewValleyMod.SaveModInfo.Patcher;

internal class SaveFileSlotPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<SaveFileSlot>("drawSlotName"),
            postfix: this.GetHarmonyMethod(nameof(DrawSlotNamePostfix))
        );
    }

    private static void DrawSlotNamePostfix(SpriteBatch b, int i, SaveFileSlot __instance, LoadGameMenu ___menu)
    {
        try 
        {
            if (___menu.GetType() != typeof(LoadGameMenu)) return;

            if (!CheckModInfoHandler.CheckResult.TryGetValue(__instance.Farmer.slotName, out var checkResult)) return;
            if (string.IsNullOrEmpty(checkResult)) return;

            var position = new Vector2(___menu.slotButtons[i].bounds.X + 128 + 36 + SpriteText.getWidthOfString(__instance.Farmer.Name),
                ___menu.slotButtons[i].bounds.Y + 36 - 4);
            b.Draw(Game1.mouseCursors, position, new Rectangle(383, 495, 11, 12), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());    
        }
    }
}