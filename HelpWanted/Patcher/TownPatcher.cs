using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.HelpWanted.Manager;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class TownPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<Town>(harmony, nameof(Town.draw), PatchKind.Postfix, nameof(DrawPostfix), new[] { typeof(SpriteBatch) });
    }

    // 修改任务面板感叹号的绘制
    // 代码来源：Town.draw(SpriteBatch spriteBatch)
    private static void DrawPostfix(SpriteBatch spriteBatch)
    {
        // 只管「当天还有没有没接下的任务」：已上板但还没被接下的便签同样算，只看 QuestList 会让开过一次板子后的感叹号熄灭
        if (!VanillaQuestManager.Instance.HasPendingQuests) return;

        var yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
        spriteBatch.Draw(
            Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, new Vector2(2692f, 3528f + yOffset)),
            new Rectangle(395, 497, 3, 8),
            Color.White,
            0f,
            new Vector2(1f, 4f),
            4f + Math.Max(0f, 0.25f - yOffset / 16f),
            SpriteEffects.None,
            1f
        );
    }
}
