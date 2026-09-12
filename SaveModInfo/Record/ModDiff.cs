using System;
using System.Collections.Generic;

namespace weizinai.StardewValleyMod.SaveModInfo.Record;

/// <summary>
/// 记录与当前模组集合的差异结果：三类变化（已移除 / 新增 / 已更新）的明细，外加记录元信息与「有没有记录过」。
/// </summary>
/// <remarks>
///     <para>
///     纯数据（由 <see cref="RecordDiff.Compare" /> 产出），不含磁盘与游戏访问：表现层只读它。
///     </para>
///     <para>
///     记录元信息（<see cref="RecordedAt" /> / <see cref="GameVersion" /> / <see cref="RecordedModCount" />）只在
///     <see cref="HasRecord" /> 为 true 时有意义：没有记录时界面只说明无法对比，不显示它们，因此这类实例上它们保持默认值。
///     </para>
/// </remarks>
internal class ModDiff
{
    /// <summary>已移除（记录里有、现在没有）的模组显示名，按显示名排序。</summary>
    public IReadOnlyList<string> Removed { get; init; } = Array.Empty<string>();

    /// <summary>新增（现在有、记录里没有）的模组显示名，按显示名排序。</summary>
    public IReadOnlyList<string> Added { get; init; } = Array.Empty<string>();

    /// <summary>已更新（两边都有但版本号不同）的模组，按显示名排序。</summary>
    public IReadOnlyList<ModVersionChange> Updated { get; init; } = Array.Empty<ModVersionChange>();

    /// <summary>这个存档有没有记录过模组信息（false = 记录不存在/为空/损坏，界面须说明无法对比）。</summary>
    public bool HasRecord { get; init; }

    /// <summary>记录写下的时间（<c>yyyy-MM-dd HH:mm</c>）；<see cref="HasRecord" /> 为 false 时无意义。</summary>
    public string RecordedAt { get; init; } = string.Empty;

    /// <summary>记录时的游戏版本；<see cref="HasRecord" /> 为 false 时无意义。</summary>
    public string GameVersion { get; init; } = string.Empty;

    /// <summary>记录时的模组数；<see cref="HasRecord" /> 为 false 时无意义。</summary>
    public int RecordedModCount { get; init; }

    /// <summary>当前的模组数。</summary>
    public int CurrentModCount { get; init; }

    /// <summary>三类变化里是否至少有一类有内容；与记录完全一致时三类都为空。</summary>
    public bool HasChanges => this.Removed.Count > 0 || this.Added.Count > 0 || this.Updated.Count > 0;
}
