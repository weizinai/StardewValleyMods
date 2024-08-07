using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.TestMod.Framework;
using weizinai.StardewValleyMod.TestMod.Patcher;

namespace weizinai.StardewValleyMod.TestMod;

public class ModEntry : Mod
{
    private readonly KeybindList testKet = new(SButton.O);

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        ModConfig.Init(helper);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new CalicoJackPatcher());
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForTestMod(
            new GenericModConfigMenuIntegration<ModConfig>(
                this.Helper.ModRegistry,
                this.ModManifest,
                () => ModConfig.Instance,
                () =>
                {
                    ModConfig.Instance = new ModConfig();
                    this.Helper.WriteConfig(ModConfig.Instance);
                },
                () => this.Helper.WriteConfig(ModConfig.Instance))
        ).Register();
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKet.JustPressed()) { }
    }
}