using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class GenericModConfigMenuForSpectatorMode
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuForSpectatorMode(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        this.configMenu = configMenu;
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            // 一般设置
            .AddSectionTitle(I18n.Config_GeneralSettingTitle_Name)
            .AddBoolOption(
                config => config.ShowTimeAndMoney,
                (config, value) => config.ShowTimeAndMoney = value,
                I18n.Config_ShowTimeAndMoney_Name
            )
            .AddBoolOption(
                config => config.ShowToolbar,
                (config, value) => config.ShowToolbar = value,
                I18n.Config_ShowToolbar_Name
            )
            // 旁观地点
            .AddSectionTitle(I18n.Config_SpectateLocationTitle_Name)
            .AddKeybindList(
                config => config.SpectateLocationKey,
                (config, value) => config.SpectateLocationKey = value,
                I18n.Config_SpectateLocationKey_Name
            )
            .AddBoolOption(
                config => config.OnlyShowOutdoors,
                (config, value) => config.OnlyShowOutdoors = value,
                I18n.Config_OnlyShowOutdoors_Name
            )
            .AddNumberOption(
                config => config.MoveSpeed,
                (config, value) => config.MoveSpeed = value,
                I18n.Config_MoveSpeed_Name
            )
            .AddNumberOption(
                config => config.MoveThreshold,
                (config, value) => config.MoveThreshold = value,
                I18n.Config_MoveThreshold_Name
            )
            // 旁观玩家
            .AddSectionTitle(I18n.Config_SpectatePlayerTitle_Name)
            .AddKeybindList(
                config => config.SpectatePlayerKey,
                (config, value) => config.SpectatePlayerKey = value,
                I18n.Config_SpectatePlayerKey_Name
            )
            .AddKeybindList(
                config => config.ToggleStateKey,
                (config, value) => config.ToggleStateKey = value,
                I18n.Config_ToggleStateKey_Name
            )
            // 随机旁观
            .AddSectionTitle(I18n.Config_RandomSpectateTitle_Name)
            .AddKeybindList(
                config => config.RandomSpectateKey,
                (config, value) => config.RandomSpectateKey = value,
                I18n.Config_RandomSpectateKey_Name
            )
            .AddNumberOption(
                config => config.RandomSpectateInterval,
                (config, value) => config.RandomSpectateInterval = value,
                I18n.Config_RandomSpectateInterval_Name
            );
    }
}