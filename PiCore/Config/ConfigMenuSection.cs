using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.PiCore.Config;

/// <summary>
/// 用于声明某个嵌套子配置菜单的分区构建器。选项按子配置 <typeparamref name="TSection" /> 的成员绑定，
/// 渲染时自动与“根配置到该子配置”的选择器拼接成对根配置的读写，供分区标题以外的选项复用。
/// 若同一个子配置类型在多处渲染，可把构建逻辑抽成接收本类型参数的共用方法。
/// 本类型与 <see cref="ConfigMenuDescriptor{TConfig}" /> 的选项方法一一对应（仅按子配置成员类型绑定），
/// 新增选项种类时需在两个类型上同步补方法。
/// </summary>
/// <typeparam name="TConfig">模组配置（根）类型。</typeparam>
/// <typeparam name="TSection">嵌套的子配置类型。</typeparam>
public sealed class ConfigMenuSection<TConfig, TSection> where TConfig : class, new()
{
    /// <summary>所属菜单描述器，负责存放最终渲染动作。</summary>
    private readonly ConfigMenuDescriptor<TConfig> menu;

    /// <summary>根配置到子配置的选择器表达式。</summary>
    private readonly Expression<Func<TConfig, TSection>> selector;

    /// <summary>构造一个绑定到指定根菜单与选择器的子配置分区构建器。</summary>
    /// <param name="menu">根菜单描述器。</param>
    /// <param name="selector">根配置到子配置的选择器。</param>
    internal ConfigMenuSection(ConfigMenuDescriptor<TConfig> menu, Expression<Func<TConfig, TSection>> selector)
    {
        this.menu = menu;
        this.selector = selector;
    }

    /// <summary>绑定子配置的一个布尔成员作为开关选项。</summary>
    /// <param name="member">子配置成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuSection<TConfig, TSection> AddBoolOption(
        Expression<Func<TSection, bool>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        bool enable = true
    )
    {
        this.menu.AddBoolOption(ConfigMember.Compose(this.selector, member), name, tooltip, enable);

        return this;
    }

    /// <summary>绑定子配置的一个整数成员作为数值选项。</summary>
    /// <param name="member">子配置成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="min">允许的最小值，或 <c>null</c> 不限制。</param>
    /// <param name="max">允许的最大值，或 <c>null</c> 不限制。</param>
    /// <param name="interval">可选的步进间隔。</param>
    /// <param name="formatValue">值的显示格式化，或 <c>null</c> 原样显示。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuSection<TConfig, TSection> AddNumberOption(
        Expression<Func<TSection, int>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        int? min = null,
        int? max = null,
        int? interval = null,
        Func<int, string>? formatValue = null,
        bool enable = true
    )
    {
        this.menu.AddNumberOption(ConfigMember.Compose(this.selector, member), name, tooltip, min, max, interval, formatValue, enable);

        return this;
    }

    /// <summary>绑定子配置的一个浮点成员作为数值选项。</summary>
    /// <param name="member">子配置成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="min">允许的最小值，或 <c>null</c> 不限制。</param>
    /// <param name="max">允许的最大值，或 <c>null</c> 不限制。</param>
    /// <param name="interval">可选的步进间隔。</param>
    /// <param name="formatValue">值的显示格式化，或 <c>null</c> 原样显示。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuSection<TConfig, TSection> AddNumberOption(
        Expression<Func<TSection, float>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        float? min = null,
        float? max = null,
        float? interval = null,
        Func<float, string>? formatValue = null,
        bool enable = true
    )
    {
        this.menu.AddNumberOption(ConfigMember.Compose(this.selector, member), name, tooltip, min, max, interval, formatValue, enable);

        return this;
    }

    /// <summary>绑定子配置的一个字符串成员作为文本选项。</summary>
    /// <param name="member">子配置成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="allowedValues">可选值列表，或 <c>null</c> 允许任意文本。</param>
    /// <param name="formatAllowedValue">可选值的显示格式化，或 <c>null</c> 原样显示。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuSection<TConfig, TSection> AddTextOption(
        Expression<Func<TSection, string>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        Func<string[]>? allowedValues = null,
        Func<string, string>? formatAllowedValue = null,
        bool enable = true
    )
    {
        this.menu.AddTextOption(ConfigMember.Compose(this.selector, member), name, tooltip, allowedValues, formatAllowedValue, enable);

        return this;
    }

    /// <summary>绑定子配置的一个枚举成员，以文本选项的形式呈现。</summary>
    /// <typeparam name="TEnum">枚举类型。</typeparam>
    /// <param name="member">子配置成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="allowedValues">允许的枚举值（决定顺序），或 <c>null</c> 使用全部枚举值。</param>
    /// <param name="formatValue">每个枚举值的显示文本（本地化），或 <c>null</c> 显示成员名。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuSection<TConfig, TSection> AddEnumOption<TEnum>(
        Expression<Func<TSection, TEnum>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        IEnumerable<TEnum>? allowedValues = null,
        Func<TEnum, string>? formatValue = null,
        bool enable = true
    )
        where TEnum : struct, Enum
    {
        this.menu.AddEnumOption(ConfigMember.Compose(this.selector, member), name, tooltip, allowedValues, formatValue, enable);

        return this;
    }

    /// <summary>绑定子配置的一个 <see cref="KeybindList" /> 成员作为按键绑定列表选项。</summary>
    /// <param name="member">子配置成员访问表达式。</param>
    /// <param name="name">选项标签。</param>
    /// <param name="tooltip">悬停提示，或 <c>null</c> 以禁用。</param>
    /// <param name="enable">是否添加该选项。</param>
    public ConfigMenuSection<TConfig, TSection> AddKeybindListOption(
        Expression<Func<TSection, KeybindList>> member,
        Func<string> name,
        Func<string>? tooltip = null,
        bool enable = true
    )
    {
        this.menu.AddKeybindListOption(ConfigMember.Compose(this.selector, member), name, tooltip, enable);

        return this;
    }
}
