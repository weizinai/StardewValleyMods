using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.SaveModInfo.Handler;
using static StardewValley.Menus.LoadGameMenu;

namespace weizinai.StardewValleyMod.SaveModInfo.Patcher;

internal class LoadGameMenuPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<LoadGameMenu>(nameof(LoadGameMenu.performHoverAction)),
            postfix: this.GetHarmonyMethod(nameof(PerformHoverActionPostfix))
        );
    }

    private static void PerformHoverActionPostfix(int x, int y, LoadGameMenu __instance, ref string ___hoverText)
    {
        if (__instance.GetType() != typeof(LoadGameMenu)) return;

        for (var i = 0; i < __instance.slotButtons.Count; i++)
        {
            if (__instance.currentItemIndex + i < __instance.MenuSlots.Count)
            {
                var farmer = (__instance.MenuSlots[__instance.currentItemIndex + i] as SaveFileSlot)!.Farmer;
                var bound = new Rectangle(__instance.slotButtons[i].bounds.X + 128 + 36 + SpriteText.getWidthOfString(farmer.Name),
                    __instance.slotButtons[i].bounds.Y + 36 - 4, 44, 48);

                if (bound.Contains(x, y))
                {
                    ___hoverText = CheckModInfoHandler.CheckResult[farmer.slotName];
                    break;
                }
            }
        }
    }
}