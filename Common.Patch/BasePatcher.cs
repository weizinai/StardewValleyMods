using System.Reflection;
using HarmonyLib;

namespace Common.Patch;

public abstract class BasePatcher : IPatcher
{
    public abstract void Apply(Harmony harmony);

    /// <summary>获取构造函数并断言已找到。</summary>
    /// <typeparam name="T">包含方法的类型。</typeparam>
    /// <param name="parameters">方法参数类型，如果没有重载则为<c>null</c>。</param>
    /// <exception cref="InvalidOperationException">该类型没有匹配的构造函数。</exception>
    protected ConstructorInfo RequireConstructor<T>(Type[]? parameters = null)
    {
        return AccessTools.Constructor(typeof(T), parameters) ??
               throw new InvalidOperationException($"Can't find constructor {GetMethodString(typeof(T), null, parameters)} to patch.");
    }

    /// <summary>获取方法并断言已找到。</summary>
    /// <typeparam name="T">包含该方法的类型。</typeparam>
    /// <param name="name">方法名称。</param>
    /// <param name="parameters">方法参数类型，如果未重载，则为<c>null</c>。</param>
    /// <exception cref="InvalidOperationException">类型没有匹配的方法。</exception>W
    protected MethodInfo RequireMethod<T>(string name, Type[]? parameters = null)
    {
        return AccessTools.Method(typeof(T), name, parameters) ??
               throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(T), name, parameters)} to patch.");
    }

    /// <summary>获取当前实例上的 Harmony 方法。</summary>
    /// <param name="name">方法名称。</param>
    protected HarmonyMethod GetHarmonyMethod(string name)
    {
        return new HarmonyMethod(AccessTools.Method(GetType(), name)) ??
               throw new InvalidOperationException($"Can't find patcher method {GetMethodString(GetType(), name)}.");
    }

    /// <summary>获取方法的可读表示。</summary>
    /// <param name="type">包含方法的类型。</param>
    /// <param name="name">方法名称，或 <c>null</c> 表示构造函数。</param>
    /// <param name="parameters">方法参数类型，如果未重载则为 <c>null</c>。</param>
    protected static string GetMethodString(Type type, string? name, Type[]? parameters = null)
    {
        var paramString = parameters?.Any() == true 
            ? $"({string.Join(", ", parameters.Select(p => p.FullName))})"
            : string.Empty;

        return $"{type.FullName}{(name != null ? "." + name : string.Empty)}{paramString}";
    }
}