using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.PiCore.Extension;
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
        HarmonyPatcher.Apply(this,
            new CalicoJackPatcher(),
            new WheelSpinGamePatcher()
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            () => ModConfig.Instance,
            value => ModConfig.Instance = value,
            configMenu => configMenu
                .AddSectionTitle(I18n.Config_CardChance_Name)
                .AddBoolOption(
                    config => config.CardChance.IsEnabled,
                    (config, value) => config.CardChance.IsEnabled = value,
                    I18n.Config_IsEnabled_Name
                )
                .AddNumberOption(
                    config => config.CardChance.Value,
                    (config, value) => config.CardChance.Value = value,
                    I18n.Config_Value_Name
                )
                .AddSectionTitle(I18n.Config_WheelSpinSpeed_Name)
                .AddBoolOption(
                    config => config.WheelSpinSpeed.IsEnabled,
                    (config, value) => config.WheelSpinSpeed.IsEnabled = value,
                    I18n.Config_IsEnabled_Name
                )
                .AddNumberOption(
                    config => config.WheelSpinSpeed.Value,
                    (config, value) => config.WheelSpinSpeed.Value = value,
                    I18n.Config_Value_Name,
                    null,
                    0,
                    14
                )
                .AddBoolOption(
                    config => config.ExtraSpeed,
                    (config, value) => config.ExtraSpeed = value,
                    I18n.Config_ExtraSpeed_Name
                )
        );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKey1.JustPressed()) { }

        if (this.testKey2.JustPressed()) { }
    }
}
