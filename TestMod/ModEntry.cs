using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.TestMod.Config;
using weizinai.StardewValleyMod.TestMod.Patcher;

namespace weizinai.StardewValleyMod.TestMod;

internal class ModEntry : Mod
{
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(this.Helper.Translation);
        // 配置模块接管读取与 GMCM 生命周期，实例写入静态 ModConfig.Instance 供补丁读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        HarmonyPatcher.Apply(
            this,
            new CalicoJackPatcher(),
            new WheelSpinGamePatcher()
        );
    }

    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddSection(
                config => config.CardChance,
                I18n.Config_CardChance,
                section => section
                    .AddBoolOption(subConfig => subConfig.IsEnabled, I18n.Config_IsEnabled)
                    .AddNumberOption(subConfig => subConfig.Value, I18n.Config_Value)
            )
            .AddSection(
                config => config.WheelSpinSpeed,
                I18n.Config_WheelSpinSpeed,
                section => section
                    .AddBoolOption(subConfig => subConfig.IsEnabled, I18n.Config_IsEnabled)
                    .AddNumberOption(subConfig => subConfig.Value, I18n.Config_Value, null, 0, 14)
            )
            .AddBoolOption(config => config.ExtraSpeed, I18n.Config_ExtraSpeed);
    }
}
