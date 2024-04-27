using Common.Patch;
using Common.UI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace AutoBreakGeode.Patches;

public class GeodeMenuPatcher : BasePatcher
{
    private static RootElement ui = null!;

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireConstructor<GeodeMenu>(),
            postfix: GetHarmonyMethod(nameof(GeodeMenuPostfix))
        );
        harmony.Patch(
            RequireMethod<GeodeMenu>(nameof(GeodeMenu.update)),
            postfix: GetHarmonyMethod(nameof(UpdatePostfix))
        );
        harmony.Patch(
            RequireMethod<GeodeMenu>(nameof(GeodeMenu.draw), new[] { typeof(SpriteBatch) }),
            postfix: GetHarmonyMethod(nameof(DrawPostfix))
        );
    }

    private static void GeodeMenuPostfix(ClickableComponent ___geodeSpot, InventoryMenu ___inventory)
    {
        ui = new RootElement();
        var button = new Button(I18n.UI_BeginButton_Name(), new Vector2(1296, 700))
        {
            OnHover = (element, _) => (element as Button)!.TextureColor = Color.White * 0.7f,
            OffHover = element => (element as Button)!.TextureColor = Color.White,
            OnLeftClick = () => ModEntry.AutoBreakGeode = !ModEntry.AutoBreakGeode
        };
        var x = ___geodeSpot.bounds.X + ___geodeSpot.bounds.Width + (___inventory.width - ___geodeSpot.bounds.Width) / 2 - button.Width / 2;
        var y = ___geodeSpot.bounds.Y + ___geodeSpot.bounds.Height * 0.75f;
        button.LocalPosition = new Vector2(x, (int)y);
        ui.AddChild(button);
    }

    private static void UpdatePostfix()
    {
        ui.Update();
        ui.ReceiveLeftClick();
    }

    private static void DrawPostfix(SpriteBatch b)
    {
        ui.Draw(b);
        ui.PerformHoverAction(b);
    }
}