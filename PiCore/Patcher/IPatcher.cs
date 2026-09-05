using HarmonyLib;

namespace weizinai.StardewValleyMod.PiCore.Patcher;

/// <summary>
/// 一组要应用到游戏的 Harmony 补丁。
/// </summary>
public interface IPatcher
{
    /// <summary>
    /// 补丁名，用于日志消息。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 该补丁是否应被应用；被禁用的补丁会被 <see cref="HarmonyPatcher" /> 跳过。
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// 应用本补丁实例的 Harmony 补丁。
    /// </summary>
    /// <param name="harmony">Harmony 实例。</param>
    public void Apply(Harmony harmony);
}
