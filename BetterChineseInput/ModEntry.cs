using System.Runtime.InteropServices;
using System.Text;
using BetterChineseInput.Framework;
using Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BetterChineseInput;

public class ModEntry : Mod
{
    private bool lastSelected;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(Monitor);
        // 注册事件
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.keyboardDispatcher?.Subscriber?.Selected == true && lastSelected == false)
        {
            InputCacheManager.CacheClear();
            Log.Info("选中了");
        }

        lastSelected = Game1.keyboardDispatcher?.Subscriber?.Selected ?? false;
    }
}