using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace weizinai.StardewValleyMod.PiCore.Patcher;

/// <summary>
/// 为 <see cref="IPatcher" /> 实例提供基础实现逻辑。
/// 补丁方法必须是静态方法（Harmony 约束），需要依赖时由子类通过静态单例持有访问。
/// </summary>
public abstract class BasePatcher : IPatcher
{
    /// <inheritdoc />
    public virtual string Name => this.GetType().Name;

    /// <inheritdoc />
    public virtual bool IsEnabled => true;

    /// <inheritdoc />
    public abstract void Apply(Harmony harmony);

    /// <summary>
    /// 挂载一个方法补丁，并断言目标方法与补丁方法都存在。
    /// 目标方法名与补丁方法名分开显式指定，避免按命名约定推导带来的隐式依赖。
    /// </summary>
    /// <typeparam name="T">目标方法所在类型。</typeparam>
    /// <param name="harmony">Harmony 实例。</param>
    /// <param name="targetMethod">目标方法名。</param>
    /// <param name="kind">补丁类型。</param>
    /// <param name="patchMethod">本补丁类上的静态补丁方法名。</param>
    /// <param name="parameters">目标方法参数类型；方法未重载时可传 <c>null</c>。</param>
    /// <exception cref="InvalidOperationException">目标方法或补丁方法未找到，或补丁方法不是静态方法。</exception>
    protected void Patch<T>(Harmony harmony, string targetMethod, PatchKind kind, string patchMethod, Type[]? parameters = null)
    {
        this.ApplyPatch(harmony, this.RequireMethod<T>(targetMethod, parameters), kind, this.GetHarmonyMethod(patchMethod));
    }

    /// <summary>
    /// 挂载一个构造器补丁，并断言目标构造器与补丁方法都存在。
    /// </summary>
    /// <typeparam name="T">目标构造器所在类型。</typeparam>
    /// <param name="harmony">Harmony 实例。</param>
    /// <param name="kind">补丁类型。</param>
    /// <param name="patchMethod">本补丁类上的静态补丁方法名。</param>
    /// <param name="parameters">构造器参数类型；构造器未重载时可传 <c>null</c>。</param>
    /// <exception cref="InvalidOperationException">目标构造器或补丁方法未找到，或补丁方法不是静态方法。</exception>
    protected void PatchConstructor<T>(Harmony harmony, PatchKind kind, string patchMethod, Type[]? parameters = null)
    {
        this.ApplyPatch(harmony, this.RequireConstructor<T>(parameters), kind, this.GetHarmonyMethod(patchMethod));
    }

    /// <summary>
    /// 获取一个构造器，并断言其存在。
    /// </summary>
    /// <typeparam name="T">构造器所在类型。</typeparam>
    /// <param name="parameters">构造器参数类型；构造器未重载时可传 <c>null</c>。</param>
    /// <exception cref="InvalidOperationException">该类型没有匹配的构造器。</exception>
    private ConstructorInfo RequireConstructor<T>(Type[]? parameters = null)
    {
        return AccessTools.Constructor(typeof(T), parameters) ??
               throw new InvalidOperationException($"Can't find constructor {GetMethodString(typeof(T), null, parameters)} to patch.");
    }

    /// <summary>
    /// 获取一个方法，并断言其存在。
    /// </summary>
    /// <typeparam name="T">方法所在类型。</typeparam>
    /// <param name="name">方法名。</param>
    /// <param name="parameters">方法参数类型；方法未重载时可传 <c>null</c>。</param>
    /// <exception cref="InvalidOperationException">该类型没有匹配的方法。</exception>
    private MethodInfo RequireMethod<T>(string name, Type[]? parameters = null)
    {
        return AccessTools.Method(typeof(T), name, parameters) ??
               throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(T), name, parameters)} to patch.");
    }

    /// <summary>
    /// 获取一个属性 getter，并断言其存在。
    /// </summary>
    /// <typeparam name="T">属性所在类型。</typeparam>
    /// <param name="name">属性名。</param>
    /// <exception cref="InvalidOperationException">该类型没有匹配的属性 getter。</exception>
    protected MethodInfo RequirePropertyGetter<T>(string name)
    {
        return AccessTools.Property(typeof(T), name)?.GetGetMethod() ??
               throw new InvalidOperationException($"Can't find property getter {GetMethodString(typeof(T), name)} to patch.");
    }

    /// <summary>
    /// 获取一个属性 setter，并断言其存在。
    /// </summary>
    /// <typeparam name="T">属性所在类型。</typeparam>
    /// <param name="name">属性名。</param>
    /// <exception cref="InvalidOperationException">该类型没有匹配的属性 setter。</exception>
    protected MethodInfo RequirePropertySetter<T>(string name)
    {
        return AccessTools.Property(typeof(T), name)?.GetSetMethod() ??
               throw new InvalidOperationException($"Can't find property setter {GetMethodString(typeof(T), name)} to patch.");
    }

    /// <summary>
    /// 获取本补丁类上的 Harmony 补丁方法，并断言其存在且为静态方法。
    /// </summary>
    /// <param name="name">方法名。</param>
    /// <exception cref="InvalidOperationException">方法未找到，或不是静态方法。</exception>
    protected HarmonyMethod GetHarmonyMethod(string name)
    {
        var method = AccessTools.Method(this.GetType(), name) ??
                     throw new InvalidOperationException($"Can't find method {GetMethodString(this.GetType(), name)} to patch with.");

        if (!method.IsStatic)
        {
            throw new InvalidOperationException($"Harmony patch method {GetMethodString(this.GetType(), name)} must be static.");
        }

        return new HarmonyMethod(method);
    }

    /// <summary>
    /// 将补丁方法按类型挂载到目标方法上。
    /// </summary>
    /// <param name="harmony">Harmony 实例。</param>
    /// <param name="original">目标方法。</param>
    /// <param name="kind">补丁类型。</param>
    /// <param name="patch">补丁方法。</param>
    private void ApplyPatch(Harmony harmony, MethodBase original, PatchKind kind, HarmonyMethod patch)
    {
        harmony.Patch(
            original: original,
            prefix: kind == PatchKind.Prefix ? patch : null,
            postfix: kind == PatchKind.Postfix ? patch : null,
            transpiler: kind == PatchKind.Transpiler ? patch : null
        );
    }

    /// <summary>
    /// 获取方法目标的易读表示。
    /// </summary>
    /// <param name="type">方法所在类型。</param>
    /// <param name="name">方法名；构造器传 <c>null</c>。</param>
    /// <param name="parameters">方法参数类型；方法未重载时可传 <c>null</c>。</param>
    private static string GetMethodString(Type type, string? name, Type[]? parameters = null)
    {
        var paramString = parameters?.Any() == true
            ? $"({string.Join(", ", parameters.Select(p => p.FullName))})"
            : string.Empty;

        return $"{type.FullName}{(name != null ? "." + name : string.Empty)}{paramString}";
    }
}
