using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using TestMod.Framework;
using TestMod.Patcher;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace TestMod;

public class ModEntry : Mod
{
    private readonly KeybindList testKet = new(SButton.O);

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        ModConfig.Init(helper);
        // 注册事件
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new CalicoJackPatcher());
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKet.JustPressed()) { }
    }
}