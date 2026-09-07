using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.PiCore.Config;
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
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供补丁程序读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        // 注册事件
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this,
            new CalicoJackPatcher(),
            new WheelSpinGamePatcher()
        );
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddSection(
                config => config.CardChance,
                I18n.Config_CardChance_Name,
                section => section
                    .AddBoolOption(subConfig => subConfig.IsEnabled, I18n.Config_IsEnabled_Name)
                    .AddNumberOption(subConfig => subConfig.Value, I18n.Config_Value_Name)
            )
            .AddSection(
                config => config.WheelSpinSpeed,
                I18n.Config_WheelSpinSpeed_Name,
                section => section
                    .AddBoolOption(subConfig => subConfig.IsEnabled, I18n.Config_IsEnabled_Name)
                    .AddNumberOption(subConfig => subConfig.Value, I18n.Config_Value_Name, null, 0, 14)
            )
            .AddBoolOption(config => config.ExtraSpeed, I18n.Config_ExtraSpeed_Name);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKey1.JustPressed()) return;

        if (this.testKey2.JustPressed()) return;
    }
}
