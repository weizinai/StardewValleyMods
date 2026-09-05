namespace weizinai.StardewValleyMod.PiCore.Patcher;

/// <summary>
/// Harmony 补丁类型。
/// </summary>
public enum PatchKind
{
    /// <summary>
    /// 原方法执行前运行；通过返回 <c>false</c> 跳过原方法。
    /// </summary>
    Prefix,

    /// <summary>
    /// 原方法执行后运行。
    /// </summary>
    Postfix,

    /// <summary>
    /// 改写原方法的 IL 代码。
    /// </summary>
    Transpiler
}
