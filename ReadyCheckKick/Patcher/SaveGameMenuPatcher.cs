using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.ReadyCheckKick.Framework;

namespace weizinai.StardewValleyMod.ReadyCheckKick.Patcher;

internal class SaveGameMenuPatcher : BasePatcher
{
    private static IReflectionHelper helper = null!;

    public SaveGameMenuPatcher(IReflectionHelper helper)
    {
        SaveGameMenuPatcher.helper = helper;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<SaveGameMenu>(nameof(SaveGameMenu.draw), new[] { typeof(SpriteBatch) }),
            postfix: this.GetHarmonyMethod(nameof(DrawPostfix))
        );
    }

    private static void DrawPostfix(SpriteBatch b)
    {
        if (!ModConfig.Instance.ShowInfoInSaveGameMenu) return;

        var endOfNightStatus = Game1.player.team.endOfNightStatus;
        var formattedStatusList = helper.GetField<Dictionary<long, string>>(endOfNightStatus, "_formattedStatusList").GetValue();

        // 未准备玩家获取逻辑
        var unreadyFarmers = new List<string>();
        foreach (var farmer in Game1.getOnlineFarmers())
        {
            if (formattedStatusList.TryGetValue(farmer.UniqueMultiplayerID, out var status) && status != "ready")
            {
                unreadyFarmers.Add(farmer.Name);
            }
        }

        if (!unreadyFarmers.Any()) return;

        // 文字绘制逻辑
        var text = string.Join("\n", unreadyFarmers);
        var size = Game1.dialogueFont.MeasureString(text);
        var position = new Vector2(Game1.uiViewport.Width - size.X - 64, 64);
        b.DrawString(Game1.dialogueFont, text, position, Color.Red);
    }
}