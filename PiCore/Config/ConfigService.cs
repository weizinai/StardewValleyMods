using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.PiCore.Config;

/// <summary>
/// 配置模块的生命周期服务：读取（含损坏自愈）、GMCM 注册、保存与重置都由本服务承担，其中保存与重置收敛到单一 <see cref="onConfigChanged" /> 回调，供模组重建处理器。
/// 配置实例住在 <see cref="SingletonConfig{TConfig}.Instance" />，构造时读盘并写入该槽位，模组各处（含构造期注册的补丁）直接现读它。
/// 重置为就地修改：把 <c>new TConfig()</c> 的默认值写回当前实例、保持引用不变，构造期捕获了配置引用的消费者（如补丁）无需重启即可看到结果。
/// </summary>
/// <typeparam name="TConfig">模组根配置类型：继承 <see cref="SingletonConfig{TConfig}" />，且含可序列化的默认构造。</typeparam>
public sealed class ConfigService<TConfig> where TConfig : SingletonConfig<TConfig>, new()
{
    // 消费该服务的模组。
    private readonly IMod mod;

    // 配置变更回调：保存或重置后触发一次。
    private readonly Action? onConfigChanged;

    // 菜单构建委托，由模组在 Entry 时提供，游戏启动后执行。
    private Action<ConfigMenuDescriptor<TConfig>>? buildMenu;

    // 菜单是否仅限标题界面编辑。
    private bool titleScreenOnly;

    // GameLaunched 是否已触发。
    private bool launched;

    // 当前持有的 GMCM 集成句柄，供打开/卸载/重载菜单使用。
    private GenericModConfigMenuIntegration<TConfig>? configMenu;

    /// <summary>构造生命周期服务：立即读盘（损坏则自愈重置）写入 <see cref="SingletonConfig{TConfig}.Instance" />，并订阅 GameLaunched 以按需注册 GMCM 菜单。</summary>
    /// <param name="mod">消费该服务的模组入口实例。</param>
    /// <param name="onConfigChanged">配置变更回调：保存或重置后触发，或 <c>null</c>。</param>
    public ConfigService(IMod mod, Action? onConfigChanged = null)
    {
        this.mod = mod;
        this.onConfigChanged = onConfigChanged;

        SingletonConfig<TConfig>.Instance = this.ReadConfigWithSelfHeal();
        mod.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    /// <summary>注册本模组的 GMCM 菜单；GameLaunched 已触发则立即（重新）注册，否则延后到触发时。</summary>
    /// <param name="buildMenu">用声明式描述器构建菜单的委托。</param>
    /// <param name="titleScreenOnly">菜单是否仅限标题界面编辑。</param>
    public void RegisterMenu(Action<ConfigMenuDescriptor<TConfig>> buildMenu, bool titleScreenOnly = false)
    {
        this.buildMenu = buildMenu;
        this.titleScreenOnly = titleScreenOnly;

        if (this.launched) this.RegisterMenuNow();
    }

    /// <summary>打开本模组的 GMCM 配置菜单（供热键驱动打开）。GMCM 未安装时不做任何事。</summary>
    public void OpenMenu()
    {
        this.configMenu?.OpenModMenu();
    }

    /// <summary>卸载并重新注册本模组的 GMCM 菜单：控制台命令改完动态配置（如模组列表键）后调用，会重跑构建委托以刷新 allowedValues。</summary>
    public void ReloadMenu()
    {
        this.configMenu?.Unregister();
        this.configMenu = null;

        if (this.launched && this.buildMenu is not null) this.RegisterMenuNow();
    }

    /// <summary>保存当前配置到 config.json 并触发 <see cref="onConfigChanged" />。</summary>
    public void Save()
    {
        this.mod.Helper.WriteConfig(SingletonConfig<TConfig>.Instance);
        this.onConfigChanged?.Invoke();
    }

    /// <summary>把当前配置重置为 <c>new TConfig()</c> 的默认值：默认值就地写回当前实例（引用不变），保存并触发 <see cref="onConfigChanged" />。</summary>
    public void Reset()
    {
        var target = SingletonConfig<TConfig>.Instance;
        var defaults = new TConfig();

        CopyMemberValues(defaults, target);

        this.mod.Helper.WriteConfig(target);
        this.onConfigChanged?.Invoke();
    }

    // 读取配置；读取失败（config.json 损坏）时写入默认配置并重读，保证模组可启动。
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

    // GameLaunched 触发：执行注册逻辑。
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.launched = true;

        if (this.buildMenu is not null) this.RegisterMenuNow();
    }

    // 构建描述器并把渲染动作逐个映射到 GMCM 集成对象；GMCM 未安装时静默跳过。
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
            () => SingletonConfig<TConfig>.Instance,
            this.Reset,
            this.Save
        );

        // GMCM 未安装或版本过低时不注册，配置的读写/自愈/重置仍照常工作
        if (!integration.IsLoaded) return;

        integration.Register(this.titleScreenOnly);

        foreach (var action in descriptor.Actions) action(integration);

        this.configMenu = integration;
    }

    // 把 source 的公共可写属性值复制到 target，保持 target 引用不变。
    private static void CopyMemberValues(TConfig source, TConfig target)
    {
        var type = typeof(TConfig);

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            if (property.SetMethod?.IsPublic == true) property.SetValue(target, property.GetValue(source));
    }
}
