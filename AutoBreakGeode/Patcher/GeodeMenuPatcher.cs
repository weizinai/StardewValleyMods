using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using weizinai.StardewValleyMod.AutoBreakGeode.Framework;
using weizinai.StardewValleyMod.AutoBreakGeode.Framework.UI;
using weizinai.StardewValleyMod.Common.UI;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Patcher;

internal class GeodeMenuPatcher : BasePatcher
{
    private static GeodeMenuPatcher instance = null!;
    private readonly ModConfig config;
    private static RootElement? ui;

    public GeodeMenuPatcher(ModConfig config)
    {
        if (instance != null)
        {
            throw new InvalidOperationException($"{nameof(GeodeMenuPatcher)} already initialized.");
        }

        this.config = config;
        instance = this;
    }

    public override void Apply(Harmony harmony)
    {
        this.PatchConstructor<GeodeMenu>(harmony, PatchKind.Postfix, nameof(GeodeMenuPostfix));
        this.Patch<GeodeMenu>(harmony, nameof(GeodeMenu.update), PatchKind.Postfix, nameof(UpdatePostfix));
        this.Patch<GeodeMenu>(harmony, nameof(GeodeMenu.draw), PatchKind.Transpiler, nameof(DrawTranspiler), new[] { typeof(SpriteBatch) });
    }

    private static void GeodeMenuPostfix(ClickableComponent ___geodeSpot)
    {
        if (!instance.config.DrawBeginButton)
        {
            return;
        }

        ui = new RootElement();
        var button = new Button(I18n.UI_BeginButton_Name(), Vector2.Zero)
        {
            OnHover = (element, _) => (element as Button)!.TextureColor = Color.White * 0.7f,
            OffHover = element => (element as Button)!.TextureColor = Color.White,
            OnLeftClick = () => ModEntry.AutoBreakGeode = !ModEntry.AutoBreakGeode
        };
        var x = ___geodeSpot.bounds.X;
        var y = ___geodeSpot.bounds.Y;
        button.LocalPosition = new Vector2(x, y);
        ui.AddChild(button);
    }

    private static void UpdatePostfix()
    {
        if (ui is null)
        {
            return;
        }

        ui.Update();
        ui.ReceiveLeftClick();
    }

    private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var parameters = new[]
        {
            typeof(Texture2D), typeof(Vector2), typeof(Rectangle), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects),
            typeof(float)
        };
        var index = codes.FindIndex(code =>
            code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), parameters)));
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GeodeMenuPatcher), nameof(DrawButton))));

        return codes.AsEnumerable();
    }

    private static void DrawButton(SpriteBatch spriteBatch)
    {
        if (ui is null)
        {
            return;
        }

        ui.Draw(spriteBatch);
        ui.PerformHoverAction(spriteBatch);
    }
}
