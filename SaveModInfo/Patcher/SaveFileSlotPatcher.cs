using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SaveModInfo.Handler;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Patcher;
using static StardewValley.Menus.LoadGameMenu;

namespace SaveModInfo.Patcher;

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
        if(___menu.GetType() != typeof(LoadGameMenu)) return;
        
        if (string.IsNullOrEmpty(CheckModInfoHandler.CheckResult[__instance.Farmer.slotName])) return;
        
        var position = new Vector2(___menu.slotButtons[i].bounds.X + 128 + 36 + SpriteText.getWidthOfString(__instance.Farmer.Name), 
            ___menu.slotButtons[i].bounds.Y + 36 - 4);
        b.Draw(Game1.mouseCursors, position, new Rectangle(383,495, 11, 12), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
    }
}