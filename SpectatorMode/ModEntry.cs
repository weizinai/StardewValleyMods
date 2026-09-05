using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.SpectatorMode.Framework;
using weizinai.StardewValleyMod.SpectatorMode.Handler;

namespace weizinai.StardewValleyMod.SpectatorMode;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        ModConfig.Init(helper);
        this.InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            () => ModConfig.Instance,
            config => ModConfig.Instance = config,
            configMenu => configMenu
                // 一般设置
                .AddSectionTitle(I18n.Config_GeneralSettingTitle_Name)
                .AddBoolOption(
                    config => config.ShowSpectateTooltip,
                    (config, value) => config.ShowSpectateTooltip = value,
                    I18n.Config_ShowSpectateTooltip_Name
                )
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
                .AddBoolOption(
                    config => config.AutoSpectatePlayer,
                    (config, value) => config.AutoSpectatePlayer = value,
                    I18n.Config_AutoSpectatePlayer_Name
                )
                .AddNumberOption(
                    config => config.AutoSpectatePlayerTime,
                    (config, value) => config.AutoSpectatePlayerTime = value,
                    I18n.Config_AutoSpectatePlayerTime_Name
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
                )
                .AddBoolOption(
                    config => config.ShowRandomSpectateTooltip,
                    (config, value) => config.ShowRandomSpectateTooltip = value,
                    I18n.Config_ShowRandomSpectateTooltip_Name
                )
                // 自动过夜
                .AddSectionTitle(I18n.Config_AutoSleep_Name)
                .AddBoolOption(
                    config => config.AutoSleep,
                    (config, value) => config.AutoSleep = value,
                    I18n.Config_AutoSleep_Name
                )
                .AddNumberOption(
                    config => config.AutoSleepTime,
                    (config, value) => config.AutoSleepTime = value,
                    I18n.Config_AutoSleepTime_Name
                )
                .AddBoolOption(
                    config => config.SkipShippingMenu,
                    (config, value) => config.SkipShippingMenu = value,
                    I18n.Config_SkipShippingMenu_Name
                )
                // 自动节日
                .AddSectionTitle(I18n.Config_AutoFestivalTitle_Name)
                .AddBoolOption(
                    config => config.AutoParticipateFestival,
                    (config, value) => config.AutoParticipateFestival = value,
                    I18n.Config_AutoParticipateFestival_Name
                )
                // 自动事情
                .AddSectionTitle(I18n.Config_AutoSkipEvent_Name)
                .AddBoolOption(
                    config => config.AutoSkipEvent,
                    (config, value) => config.AutoSkipEvent = value,
                    I18n.Config_AutoSkipEvent_Name
                )
        );
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new AutoEventHandler(this.Helper),
            new AutoFestivalHandler(this.Helper),
            new AutoSleepHandler(this.Helper),
            new CommandHandler(this.Helper),
            new SpectateLocationHandler(this.Helper),
            new SpectatePlayerHandler(this.Helper)
        };

        foreach (var handler in handlers)
        {
            handler.Apply();
        }
    }
}
