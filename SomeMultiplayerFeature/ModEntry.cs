using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Multiplayer;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature;

internal class ModEntry : Mod
{
    public const string ModDataPrefix = "weizinai.SMF.";

    private ConfigService<ModConfig> configService = null!;

    private IHandler[] handlers = Array.Empty<IHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Logger<ModEntry>.Init(this);
        Broadcaster<ModEntry>.Init(this);
        // 热键打开菜单需要服务句柄，保存/重置后经回调重建处理器
        this.configService = new ConfigService<ModConfig>(this, this.UpdateConfig);
        this.configService.RegisterMenu(this.BuildConfigMenu);
        // 注册事件
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        this.UpdateConfig();
        // 注册Harmony补丁
        HarmonyPatcher.Apply(
            this,
            new FarmerPatcher(),
            new FarmHousePatcher(),
            new Game1Patcher()
        );
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。本模组无 i18n，标签沿用原硬编码中文文案。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddKeybindListOption(config => config.OpenConfigMenuKey, () => "打开配置菜单快捷键")
            // 自动设置Ip连接
            .AddBoolSection(config => config.AutoSetIpConnection, () => "自动设置Ip连接")
            .AddNumberOption(config => config.EnableTime, () => "启用时间", null, 6, 26)
            .AddNumberOption(config => config.DisableTime, () => "禁用时间", null, 6, 26)
            // 显示玩家数量
            .AddBoolSection(config => config.ShowPlayerCount, () => "显示玩家数量")
            // 显示提示
            .AddBoolSection(config => config.ShowTip, () => "显示提示")
            .AddTextOption(config => config.TipText, () => "提示内容")
            // 版本限制
            .AddBoolSection(config => config.VersionLimit, () => "版本限制")
            .AddNumberOption(config => config.KickPlayerDelayTime, () => "踢出玩家延迟时间");
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModConfig.Instance.OpenConfigMenuKey.JustPressed())
        {
            this.configService.OpenMenu();
        }
    }

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers)
        {
            handler.Clear();
        }

        this.handlers = new IHandler[]
        {
            new AutoClickHandler(this.Helper),
            new CustomCommandHandler(this.Helper),
            new DataHandler(this.Helper),
            new IpConnectionHandler(this.Helper),
            new PerfectFishingHandler(this.Helper),
            new PlayerCountHandler(this.Helper),
            new TipHandler(this.Helper),
            new VersionLimitHandler(this.Helper)
        };

        foreach (var handler in this.handlers)
        {
            handler.Apply();
        }
    }
}
