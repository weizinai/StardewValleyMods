using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Handler;
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
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供各处理器读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        this.InitHandler();
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            // 一般设置
            .AddSectionTitle(I18n.Config_GeneralSettingTitle_Name)
            .AddBoolOption(config => config.ShowSpectateTooltip, I18n.Config_ShowSpectateTooltip_Name)
            .AddBoolOption(config => config.ShowTimeAndMoney, I18n.Config_ShowTimeAndMoney_Name)
            .AddBoolOption(config => config.ShowToolbar, I18n.Config_ShowToolbar_Name)
            // 旁观地点
            .AddSectionTitle(I18n.Config_SpectateLocationTitle_Name)
            .AddKeybindListOption(config => config.SpectateLocationKey, I18n.Config_SpectateLocationKey_Name)
            .AddBoolOption(config => config.OnlyShowOutdoors, I18n.Config_OnlyShowOutdoors_Name)
            .AddNumberOption(config => config.MoveSpeed, I18n.Config_MoveSpeed_Name)
            .AddNumberOption(config => config.MoveThreshold, I18n.Config_MoveThreshold_Name)
            // 旁观玩家
            .AddSectionTitle(I18n.Config_SpectatePlayerTitle_Name)
            .AddKeybindListOption(config => config.SpectatePlayerKey, I18n.Config_SpectatePlayerKey_Name)
            .AddKeybindListOption(config => config.ToggleStateKey, I18n.Config_ToggleStateKey_Name)
            .AddBoolOption(config => config.AutoSpectatePlayer, I18n.Config_AutoSpectatePlayer_Name)
            .AddNumberOption(config => config.AutoSpectatePlayerTime, I18n.Config_AutoSpectatePlayerTime_Name)
            // 随机旁观
            .AddSectionTitle(I18n.Config_RandomSpectateTitle_Name)
            .AddKeybindListOption(config => config.RandomSpectateKey, I18n.Config_RandomSpectateKey_Name)
            .AddNumberOption(config => config.RandomSpectateInterval, I18n.Config_RandomSpectateInterval_Name)
            .AddBoolOption(config => config.ShowRandomSpectateTooltip, I18n.Config_ShowRandomSpectateTooltip_Name)
            // 自动过夜
            .AddBoolSection(config => config.AutoSleep, I18n.Config_AutoSleep_Name)
            .AddNumberOption(config => config.AutoSleepTime, I18n.Config_AutoSleepTime_Name)
            .AddBoolOption(config => config.SkipShippingMenu, I18n.Config_SkipShippingMenu_Name)
            // 自动节日
            .AddSectionTitle(I18n.Config_AutoFestivalTitle_Name)
            .AddBoolOption(config => config.AutoParticipateFestival, I18n.Config_AutoParticipateFestival_Name)
            // 自动跳过事件
            .AddBoolSection(config => config.AutoSkipEvent, I18n.Config_AutoSkipEvent_Name);
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

        foreach (var handler in handlers) handler.Apply();
    }
}
