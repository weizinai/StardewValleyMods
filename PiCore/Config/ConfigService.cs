using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.PiCore.Config;

/// <summary>
/// 配置模块的生命周期服务：统一负责配置读取（含损坏自愈重置）、GMCM 注册、保存与重置，
/// 并把“保存”与“重置”都收敛到单一 <see cref="onConfigChanged" /> 回调，供模组重建处理器。
/// 重置为“就地修改”：把 <c>new TConfig()</c> 的默认值写回当前实例，保持对象引用不变，
/// 因此构造期捕获了配置引用的消费者（如 AutoBreakGeode / FriendshipDecayModify 的补丁）无需重启即可看到重置结果。
/// </summary>
/// <typeparam name="TConfig">模组配置类型（含可序列化的默认构造）。</typeparam>
public sealed class ConfigService<TConfig> where TConfig : class, new()
{
    /// <summary>消费该服务的模组。</summary>
    private readonly IMod mod;

    /// <summary>取当前配置实例（模组通常绑定到静态 <c>ModConfig.Instance</c>）。</summary>
    private readonly Func<TConfig> getConfig;

    /// <summary>配置变更回调：保存或重置后触发一次。</summary>
    private readonly Action? onConfigChanged;

    /// <summary>菜单构建委托，由模组在 <c>Entry</c> 时提供，游戏启动后执行。</summary>
    private Action<ConfigMenuDescriptor<TConfig>>? buildMenu;

    /// <summary>菜单是否仅限标题界面编辑。</summary>
    private bool titleScreenOnly;

    /// <summary>GameLaunched 是否已触发。</summary>
    private bool launched;

    /// <summary>当前持有的 GMCM 集成句柄，供打开/卸载/重载菜单使用。</summary>
    private GenericModConfigMenuIntegration<TConfig>? configMenu;

    /// <summary>
    /// 构造生命周期服务：立即读取配置（损坏则自愈重置），并把读到的实例写入模组持有的位置，
    /// 同时订阅 GameLaunched 以便按需注册 GMCM 菜单。
    /// </summary>
    /// <param name="mod">消费该服务的模组入口实例。</param>
    /// <param name="getConfig">取当前配置实例的委托。</param>
    /// <param name="setConfig">写入（初始读取到的）配置实例的委托。</param>
    /// <param name="onConfigChanged">配置变更回调：保存或重置后触发，或 <c>null</c>。</param>
    public ConfigService(IMod mod, Func<TConfig> getConfig, Action<TConfig> setConfig, Action? onConfigChanged = null)
    {
        this.mod = mod;
        this.getConfig = getConfig;
        this.onConfigChanged = onConfigChanged;

        setConfig(this.ReadConfigWithSelfHeal());
        mod.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    /// <summary>
    /// 注册本模组的 GMCM 菜单。若 GameLaunched 尚未触发则延迟到触发时注册；
    /// 若已触发则立即（重新）注册。
    /// </summary>
    /// <param name="buildMenu">用声明式描述器构建菜单的委托。</param>
    /// <param name="titleScreenOnly">菜单是否仅限标题界面编辑。</param>
    public void RegisterMenu(Action<ConfigMenuDescriptor<TConfig>> buildMenu, bool titleScreenOnly = false)
    {
        this.buildMenu = buildMenu;
        this.titleScreenOnly = titleScreenOnly;

        if (this.launched)
        {
            this.RegisterMenuNow();
        }
    }

    /// <summary>
    /// 打开本模组的 GMCM 配置菜单（供热键驱动打开）。GMCM 未安装时不做任何事。
    /// </summary>
    public void OpenMenu()
    {
        this.configMenu?.OpenModMenu();
    }

    /// <summary>
    /// 卸载并重新注册本模组的 GMCM 菜单。控制台命令改完动态配置（如模组列表键）后调用，
    /// 会重跑菜单构建委托以刷新 allowedValues。
    /// </summary>
    public void ReloadMenu()
    {
        this.configMenu?.Unregister();
        this.configMenu = null;

        if (this.launched && this.buildMenu is not null)
        {
            this.RegisterMenuNow();
        }
    }

    /// <summary>保存当前配置到 config.json 并触发 <see cref="onConfigChanged" />。</summary>
    public void Save()
    {
        this.mod.Helper.WriteConfig(this.getConfig());
        this.onConfigChanged?.Invoke();
    }

    /// <summary>
    /// 把当前配置重置为 <c>new TConfig()</c> 的默认值：把公共可写属性写回当前实例（不更换实例引用），
    /// 保存配置并触发 <see cref="onConfigChanged" />。
    /// </summary>
    public void Reset()
    {
        var target = this.getConfig();
        var defaults = new TConfig();

        CopyMemberValues(defaults, target);

        this.mod.Helper.WriteConfig(target);
        this.onConfigChanged?.Invoke();
    }

    /// <summary>读取配置；读取失败（config.json 损坏）时写入默认配置并重读，保证模组可启动。</summary>
    /// <returns>读取到的配置实例；损坏时返回重置后的默认配置。</returns>
    private TConfig ReadConfigWithSelfHeal()
    {
        // config.json 由玩家手写，任何解析/绑定异常都应自愈而不是让模组崩溃
        try
        {
            return this.mod.Helper.ReadConfig<TConfig>();
        }
        catch (Exception)
        {
            this.mod.Helper.WriteConfig(new TConfig());
            this.mod.Monitor.Log("Failed to read config.json and it was reset to defaults. Please re-enable the features you want.", LogLevel.Warn);

            return this.mod.Helper.ReadConfig<TConfig>();
        }
    }

    /// <summary>GameLaunched 触发：执行注册逻辑。</summary>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.launched = true;

        if (this.buildMenu is not null)
        {
            this.RegisterMenuNow();
        }
    }

    /// <summary>构建描述器并把渲染动作逐个映射到 GMCM 集成对象；GMCM 未安装时静默跳过。</summary>
    private void RegisterMenuNow()
    {
        // 先卸载旧菜单再注册，保证重复注册（GameLaunched 与模组自身 OnGameLaunched 都调用时）不触发 GMCM 的重复注册报错
        this.configMenu?.Unregister();
        this.configMenu = null;

        var descriptor = new ConfigMenuDescriptor<TConfig>();
        this.buildMenu!(descriptor);

        var integration = new GenericModConfigMenuIntegration<TConfig>(
            this.mod.Helper.ModRegistry,
            this.mod.Monitor,
            this.mod.ModManifest,
            this.getConfig,
            this.Reset,
            this.Save
        );

        // GMCM 未安装或版本过低时不注册，配置的读写/自愈/重置仍照常工作
        if (!integration.IsLoaded)
        {
            return;
        }

        integration.Register(this.titleScreenOnly);

        foreach (var action in descriptor.Actions)
        {
            action(integration);
        }

        this.configMenu = integration;
    }

    /// <summary>把 <paramref name="source" /> 的公共可写属性值复制到 <paramref name="target" />，保持 target 引用不变。</summary>
    /// <param name="source">默认值来源实例。</param>
    /// <param name="target">要被就地重置的当前实例。</param>
    private static void CopyMemberValues(TConfig source, TConfig target)
    {
        var type = typeof(TConfig);

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.SetMethod?.IsPublic == true)
            {
                property.SetValue(target, property.GetValue(source));
            }
        }
    }
}
