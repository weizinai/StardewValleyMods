using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.TestMod.Framework;
using weizinai.StardewValleyMod.TestMod.Patcher;

namespace weizinai.StardewValleyMod.TestMod;

public class ModEntry : Mod
{
    private readonly KeybindList testKey1 = new(SButton.O);
    private readonly KeybindList testKey2 = new(SButton.U);

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(this.Helper.Translation);
        ModConfig.Init(helper);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this.ModManifest.UniqueID,
            new CalicoJackPatcher(),
            new WheelSpinGamePatcher()
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForTestMod(),
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKey1.JustPressed()) { }

        if (this.testKey2.JustPressed()) { }
    }
}