using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature;

public class ModEntry : Mod
{
    public const string ModDataPrefix = "weizinai.SMF.";

    private GenericModConfigMenuIntegration<ModConfig>? configMenu;

    private IHandler[] handlers = Array.Empty<IHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Logger<ModEntry>.Init(this);
        Broadcaster<ModEntry>.Init(this);
        ModConfig.Init(helper);
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // 注册GenericModConfigMenu
        this.configMenu = this.AddGenericModConfigMenu(
            () => ModConfig.Instance,
            value => ModConfig.Instance = value,
            this.AddConfigMenu,
            this.UpdateConfig,
            this.UpdateConfig
        );

        // 注册Harmony补丁
        HarmonyPatcher.Apply(
            this,
            new FarmerPatcher(),
            new FarmHousePatcher(),
            new Game1Patcher(),
            new GameLocationPatcher()
            // new ShopMenuPatcher()
        );
    }

    private void AddConfigMenu(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu
            .AddKeybindList(
                config => config.OpenConfigMenuKey,
                (config, value) => config.OpenConfigMenuKey = value,
                () => "打开配置菜单快捷键"
            )
            // 花钱限制
            // .AddSectionTitle(() => "花钱限制")
            // .AddBoolOption(
            //     config => config.SpendLimit,
            //     (config, value) => config.SpendLimit = value,
            //     () => "花钱限制"
            // )
            // .AddNumberOption(
            //     config => config.DefaultSpendLimit,
            //     (config, value) => config.DefaultSpendLimit = value,
            //     () => "默认花钱额度"
            // )
            // .AddKeybindList(
            //     config => config.SpendLimitManagerMenuKey,
            //     (config, value) => config.SpendLimitManagerMenuKey = value,
            //     () => "花钱限制管理快捷键"
            // )
            // 自动设置Ip连接
            .AddSectionTitle(() => "自动设置Ip连接")
            .AddBoolOption(
                config => config.AutoSetIpConnection,
                (config, value) => config.AutoSetIpConnection = value,
                () => "自动设置Ip连接"
            )
            .AddNumberOption(
                config => config.EnableTime,
                (config, value) => config.EnableTime = value,
                () => "启用时间",
                null,
                6,
                26
            )
            .AddNumberOption(
                config => config.DisableTime,
                (config, value) => config.DisableTime = value,
                () => "禁用时间",
                null,
                6,
                26
            )
            // 显示玩家数量
            .AddSectionTitle(() => "显示玩家数量")
            .AddBoolOption(
                config => config.ShowPlayerCount,
                (config, value) => config.ShowPlayerCount = value,
                () => "显示玩家数量"
            )
            // 显示提示
            .AddSectionTitle(() => "显示提示")
            .AddBoolOption(
                config => config.ShowTip,
                (config, value) => config.ShowTip = value,
                () => "显示提示"
            )
            .AddTextOption(
                config => config.TipText,
                (config, value) => config.TipText = value,
                () => "提示内容"
            )
            // 踢出未准备玩家
            .AddSectionTitle(() => "踢出未准备玩家")
            .AddBoolOption(
                config => config.KickUnreadyPlayer,
                (config, value) => config.KickUnreadyPlayer = value,
                () => "踢出未准备玩家"
            )
            .AddKeybindList(
                config => config.KickUnreadyPlayerKey,
                (config, value) => config.KickUnreadyPlayerKey = value,
                () => "踢出未准备玩家快捷键"
            )
            // 版本限制
            .AddSectionTitle(() => "版本限制")
            .AddBoolOption(
                config => config.VersionLimit,
                (config, value) => config.VersionLimit = value,
                () => "版本限制"
            )
            .AddNumberOption(
                config => config.KickPlayerDelayTime,
                (config, value) => config.KickPlayerDelayTime = value,
                () => "踢出玩家延迟时间"
            );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModConfig.Instance.OpenConfigMenuKey.JustPressed())
        {
            this.configMenu?.OpenModMenu();
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
            // new SpendLimitHandler(this.Helper, this.config),
            new TipHandler(this.Helper),
            new VersionLimitHandler(this.Helper)
        };

        foreach (var handler in this.handlers)
        {
            handler.Apply();
        }
    }
}
