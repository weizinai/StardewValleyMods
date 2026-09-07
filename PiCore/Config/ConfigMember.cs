using System;
using System.Linq.Expressions;
using System.Reflection;

namespace weizinai.StardewValleyMod.PiCore.Config;

/// <summary>
/// 将配置成员访问表达式编译为 get/set 委托，或在根配置与嵌套配置之间拼接成员路径。
/// </summary>
internal static class ConfigMember
{
    /// <summary>
    /// 将表达式树中指定参数替换为另一表达式，用于拼接成员访问链。
    /// </summary>
    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression from;
        private readonly Expression to;

        public ReplaceParameterVisitor(ParameterExpression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == this.from ? this.to : base.VisitParameter(node);
        }
    }

    /// <summary>
    /// 根据成员访问表达式创建读取与写入委托。仅支持可写的公共属性成员链（配置类成员为公共自动属性），
    /// 不接受公共字段；复杂读写请走逃生舱。
    /// </summary>
    /// <typeparam name="TConfig">配置类型。</typeparam>
    /// <typeparam name="TValue">成员值类型。</typeparam>
    /// <param name="member">成员访问表达式，如 <c>config =&gt; config.Foo</c>。</param>
    /// <returns>读取与写入委托。</returns>
    internal static (Func<TConfig, TValue> get, Action<TConfig, TValue> set) CreateAccessor<TConfig, TValue>(
        Expression<Func<TConfig, TValue>> member
    )
    {
        if (member.Body is not MemberExpression) throw new ArgumentException("配置选项成员必须是一段可赋值的公共属性链，复杂读写请使用 AddCustomSection 逃生舱。", nameof(member));

        // 配置类成员形态约定为公共自动属性：成员链中任一环节是公共字段即拒绝，避免绑定层与重置层行为不一致
        if (ContainsPublicField(member.Body)) throw new ArgumentException("配置成员必须是公共自动属性，不支持公共字段（配置模块成员契约仅限可写公共属性）。", nameof(member));

        var get = member.Compile();
        var valueParameter = Expression.Parameter(typeof(TValue), "value");
        var setBody = Expression.Assign(member.Body, valueParameter);
        var set = Expression.Lambda<Action<TConfig, TValue>>(setBody, member.Parameters[0], valueParameter).Compile();

        return (get, set);
    }

    /// <summary>判断成员访问链中是否含公共字段（成员链的读取表达式或任一被访问成员是公共 <see cref="FieldInfo" />）。</summary>
    /// <param name="body">成员访问表达式的体。</param>
    private static bool ContainsPublicField(Expression body)
    {
        switch (body)
        {
            case MemberExpression memberExpression:
                return memberExpression.Member is FieldInfo field && field.IsPublic
                       || memberExpression.Expression is not null && ContainsPublicField(memberExpression.Expression);
            default:
                return false;
        }
    }

    /// <summary>
    /// 将根配置到子配置的选择器与子配置自身的成员访问表达式拼接为根配置上的成员访问表达式。
    /// </summary>
    /// <typeparam name="TRoot">根配置类型。</typeparam>
    /// <typeparam name="TSection">子配置类型。</typeparam>
    /// <typeparam name="TValue">成员值类型。</typeparam>
    /// <param name="selector">根到子配置的选择器，如 <c>config =&gt; config.Quest</c>。</param>
    /// <param name="member">子配置上的成员访问表达式，如 <c>quest =&gt; quest.Weight</c>。</param>
    /// <returns>拼接到根配置上的成员访问表达式。</returns>
    internal static Expression<Func<TRoot, TValue>> Compose<TRoot, TSection, TValue>(
        Expression<Func<TRoot, TSection>> selector,
        Expression<Func<TSection, TValue>> member
    )
    {
        var body = new ReplaceParameterVisitor(member.Parameters[0], selector.Body).Visit(member.Body);

        return Expression.Lambda<Func<TRoot, TValue>>(body, selector.Parameters[0]);
    }
}
