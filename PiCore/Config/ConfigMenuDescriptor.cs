using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.PiCore.Config;

/// <summary>
/// 以声明式、按成员绑定（无 get/set 委托对）的方式描述一个模组的 GMCM 配置菜单。
/// 菜单由若干按顺序排列的元素组成，渲染时逐个映射到 <see cref="GenericModConfigMenuIntegration{TConfig}" /> 的调用。
/// 标签与提示沿用模组强类型生成的 I18n 访问器（<c>Func&lt;string&gt;</c>），漏键仍在编译期报错。
/// </summary>
/// <typeparam name="TConfig">模组配置类型。</typeparam>
public sealed class ConfigMenuDescriptor<TConfig> where TConfig : class, new()
{
    /// <summary>按注册顺序排列的渲染动作，注册时逐个执行。请通过本类 Add* 方法追加元素，勿直接修改该列表。</summary>
    public List<Action<GenericModConfigMenuIntegration<TConfig>>> Actions { get; } = new();

    /// <summary>
    /// 在当前位置添加一个分区标题。
    /// </summary>
    /// <param name="text">标题文本。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="enable">是否添加该标题。</param>
    public ConfigMenuDescriptor<TConfig> AddSectionTitle(Func<string> text, Func<string>? tooltip = null, bool enable = true)
    {
        this.Actions.Add(menu => menu.AddSectionTitle(text, tooltip, enable));

        return this;
    }

    /// <summary>
    /// 在当前位置添加一段说明文字。
    /// </summary>
    /// <param name="text">段落文本。</param>
    /// <param name="enable">是否添加该段落。</param>
    public ConfigMenuDescriptor<TConfig> AddParagraph(Func<string> text, bool enable = true)
    {
        this.Actions.Add(menu => menu.AddParagraph(text, enable));

        return this;
    }

    /// <summary>
    /// 新增一个页面，此后的选项都归入该页，直到再次切换页面。
    /// </summary>
    /// <param name="pageId">页面唯一 ID。</param>
    /// <param name="pageTitle">页面标题，或 <c>null</c> 以显示 <paramref name="pageId" />。</param>
    public ConfigMenuDescriptor<TConfig> AddPage(string pageId, Func<string>? pageTitle = null)
    {
        this.Actions.Add(menu => menu.AddPage(pageId, pageTitle));

        return this;
    }

    /// <summary>
    /// 在当前位置添加一个跳转到指定页面的链接。
    /// </summary>
    /// <param name="pageId">目标页面唯一 ID。</param>
    /// <param name="text">链接文本。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="enable">是否添加该链接。</param>
    public ConfigMenuDescriptor<TConfig> AddPageLink(string pageId, Func<string> text, Func<string>? tooltip = null, bool enable = true)
    {
        this.Actions.Add(menu => menu.AddPageLink(pageId, text, tooltip, enable));

        return this;
    }

    /// <summary>
    /// 绑定一个布尔成员作为开关选项。
    /// </summary>
    /// <param name="member">成员访问表达式，如 <c>config =&gt; config.Enable</c>。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuDescriptor<TConfig> AddBoolOption(
        Expression<Func<TConfig, bool>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        bool enable = true
    )
    {
        var (get, set) = ConfigMember.CreateAccessor(member);
        this.Actions.Add(menu => menu.AddBoolOption(get, set, name, tooltip, enable));

        return this;
    }

    /// <summary>
    /// 添加一个由布尔开关领头的分区：开关的标签同时充当分区标题，只需声明一次。
    /// 若开关所在分区还有后续选项，可在本调用后继续链式添加。
    /// </summary>
    /// <param name="member">开关成员访问表达式。</param>
    /// <param name="name">开关标签（兼作分区标题）。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="enable">是否添加该开关。</param>
    public ConfigMenuDescriptor<TConfig> AddBoolSection(
        Expression<Func<TConfig, bool>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        bool enable = true
    )
    {
        this.AddSectionTitle(name);

        return this.AddBoolOption(member, name, tooltip, enable);
    }

    /// <summary>
    /// 绑定一个整数成员作为数值选项。
    /// </summary>
    /// <param name="member">成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="min">允许的最小值，或 <c>null</c> 不限制。</param>
    /// <param name="max">允许的最大值，或 <c>null</c> 不限制。</param>
    /// <param name="interval">可选的步进间隔。</param>
    /// <param name="formatValue">值的显示格式化，或 <c>null</c> 原样显示。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuDescriptor<TConfig> AddNumberOption(
        Expression<Func<TConfig, int>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        int? min = null,
        int? max = null,
        int? interval = null,
        Func<int, string>? formatValue = null,
        bool enable = true
    )
    {
        var (get, set) = ConfigMember.CreateAccessor(member);
        this.Actions.Add(menu => menu.AddNumberOption(get, set, name, tooltip, min, max, interval, formatValue, enable));

        return this;
    }

    /// <summary>
    /// 绑定一个浮点成员作为数值选项。
    /// </summary>
    /// <param name="member">成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="min">允许的最小值，或 <c>null</c> 不限制。</param>
    /// <param name="max">允许的最大值，或 <c>null</c> 不限制。</param>
    /// <param name="interval">可选的步进间隔。</param>
    /// <param name="formatValue">值的显示格式化，或 <c>null</c> 原样显示。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuDescriptor<TConfig> AddNumberOption(
        Expression<Func<TConfig, float>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        float? min = null,
        float? max = null,
        float? interval = null,
        Func<float, string>? formatValue = null,
        bool enable = true
    )
    {
        var (get, set) = ConfigMember.CreateAccessor(member);
        this.Actions.Add(menu => menu.AddNumberOption(get, set, name, tooltip, min, max, interval, formatValue, enable));

        return this;
    }

    /// <summary>
    /// 绑定一个字符串成员作为文本选项。可通过 <paramref name="allowedValues" /> 限定可选值，做成下拉列表。
    /// </summary>
    /// <param name="member">成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="allowedValues">可选值列表，或 <c>null</c> 允许任意文本。委托在注册时求值，便于拿到动态键集合。</param>
    /// <param name="formatAllowedValue">可选值的显示格式化，或 <c>null</c> 原样显示。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuDescriptor<TConfig> AddTextOption(
        Expression<Func<TConfig, string>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        Func<string[]>? allowedValues = null,
        Func<string, string>? formatAllowedValue = null,
        bool enable = true
    )
    {
        var (get, set) = ConfigMember.CreateAccessor(member);
        this.Actions.Add(menu => menu.AddTextOption(get, set, name, tooltip, allowedValues?.Invoke(), formatAllowedValue, enable));

        return this;
    }

    /// <summary>
    /// 绑定一个枚举成员，以文本选项的形式呈现，值用成员名映射，显示文本可本地化。
    /// </summary>
    /// <typeparam name="TEnum">枚举类型。</typeparam>
    /// <param name="member">成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="allowedValues">允许的枚举值（决定顺序），或 <c>null</c> 使用枚举声明的全部值。</param>
    /// <param name="formatValue">每个枚举值的显示文本（本地化），或 <c>null</c> 显示成员名。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuDescriptor<TConfig> AddEnumOption<TEnum>(
        Expression<Func<TConfig, TEnum>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        IEnumerable<TEnum>? allowedValues = null,
        Func<TEnum, string>? formatValue = null,
        bool enable = true
    )
        where TEnum : struct, Enum
    {
        var (get, set) = ConfigMember.CreateAccessor(member);
        var values = (allowedValues ?? Enum.GetValues<TEnum>()).ToArray();
        var valueNames = values.Select(value => value.ToString()).ToArray();
        Func<string, string>? formatAllowedValue = null;

        if (formatValue is not null)
        {
            formatAllowedValue = raw => formatValue(Enum.Parse<TEnum>(raw));
        }

        this.Actions.Add(menu => menu.AddTextOption(
            config => get(config).ToString(),
            (config, raw) => set(config, Enum.Parse<TEnum>(raw)),
            name,
            tooltip,
            valueNames,
            formatAllowedValue,
            enable
        ));

        return this;
    }

    /// <summary>
    /// 绑定一个 <see cref="KeybindList" /> 成员作为按键绑定列表选项。
    /// </summary>
    /// <param name="member">成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuDescriptor<TConfig> AddKeybindListOption(
        Expression<Func<TConfig, KeybindList>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        bool enable = true
    )
    {
        var (get, set) = ConfigMember.CreateAccessor(member);
        this.Actions.Add(menu => menu.AddKeybindList(get, set, name, tooltip, enable));

        return this;
    }

    /// <summary>
    /// 以“子配置作分区”的方式绑定一个嵌套配置对象：先按 <paramref name="title" /> 输出分区标题，
    /// 再用 <paramref name="configure" /> 声明该子配置的成员选项，作为可复用的子配置渲染单元。
    /// </summary>
    /// <typeparam name="TSection">嵌套的子配置类型。</typeparam>
    /// <param name="selector">根配置到子配置的选择器，如 <c>config =&gt; config.Quest</c>。</param>
    /// <param name="title">分区标题。</param>
    /// <param name="configure">声明子配置菜单的委托。</param>
    /// <param name="tooltip">分区标题悬停提示，或 <c>null</c> 以禁用。</param>
    public ConfigMenuDescriptor<TConfig> AddSection<TSection>(
        Expression<Func<TConfig, TSection>> selector,
        Func<string> title,
        Action<ConfigMenuSection<TConfig, TSection>> configure,
        Func<string>? tooltip = null
    )
        where TSection : class
    {
        this.AddSectionTitle(title, tooltip);

        var section = new ConfigMenuSection<TConfig, TSection>(this, selector);
        configure(section);

        return this;
    }

    /// <summary>
    /// 逃生舱：直接拿到原始 GMCM 集成对象，自行添加模块未内置的复杂选项。
    /// 适用于成长阶段字典页、动态 allowedValues 列表、列表字符串编辑等长尾场景。
    /// </summary>
    /// <param name="build">接收原始集成的自定义段构建委托。</param>
    public ConfigMenuDescriptor<TConfig> AddCustomSection(Action<GenericModConfigMenuIntegration<TConfig>> build)
    {
        this.Actions.Add(build);

        return this;
    }
}
