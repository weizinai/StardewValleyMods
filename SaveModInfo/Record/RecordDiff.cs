using System;
using System.Collections.Generic;
using System.Linq;

namespace weizinai.StardewValleyMod.SaveModInfo.Record;

/// <summary>
/// 差异计算的纯函数：输入「一份记录 + 当前模组集合」，输出差异结果。不碰磁盘、不碰游戏，可直接用手写数据穷举。
/// </summary>
internal static class RecordDiff
{
    /// <summary>显示名排序用的比较器：大小写不敏感，避免出现「大写全排前面」的怪顺序。</summary>
    private static readonly StringComparer NameComparer = StringComparer.CurrentCultureIgnoreCase;

    /// <summary>
    /// 比对记录与当前模组集合。
    /// </summary>
    /// <param name="record">存档的记录；null 或空模组表 = 没有记录过（<see cref="ModDiff.HasRecord" /> 为 false，三类列表为空）。</param>
    /// <param name="currentMods">当前已加载模组：唯一 ID → 名称与版本。</param>
    /// <returns>差异结果（三类列表各自按显示名排序，并带上记录元信息与当前模组数）。</returns>
    public static ModDiff Compare(ModInfoRecord? record, IReadOnlyDictionary<string, ModInfoEntry> currentMods)
    {
        // 「未记录」在这里收口：文件不存在与损坏的 JSON 由调用方传 null，旧形状（平字典）解析出的空模组表在此判空——
        // 空表若放过去会被算成「全部新增」。规则落在纯函数上（本功能唯一的测试接缝），调用方不必先替它归一成 null；
        // 模组表被手改成 null 的文件同样落进这一条
        if (record?.Mods is not { Count: > 0 })
        {
            return new ModDiff { HasRecord = false };
        }

        // 「已移除」= 记录里有、现在没有；「已更新」= 两边都有但版本号不同：正向遍历记录即可同时得出两者
        var removed = new List<string>();
        var updated = new List<ModVersionChange>();

        foreach (var entry in record.Mods)
        {
            if (!currentMods.TryGetValue(entry.Key, out var current))
            {
                removed.Add(entry.Value.Name);

                continue;
            }

            // 版本号按字符串精确比较：两边都来自 ModManifest.Version.ToString()，同一个版本号的写法一致。
            // 名字取**当前**的：这个模组现在就装在玩家的模组列表里，他看到的也是这个名字。
            // 只带显示名：唯一 ID 是内部标识，不给玩家看
            if (!string.Equals(entry.Value.Version, current.Version, StringComparison.Ordinal))
            {
                updated.Add(new ModVersionChange(current.Name, entry.Value.Version, current.Version));
            }
        }

        // 「新增」= 现在有、记录里没有：只能反向遍历当前模组集合
        var added = currentMods
            .Where(entry => !record.Mods.ContainsKey(entry.Key))
            .Select(entry => entry.Value.Name)
            .ToList();

        return new ModDiff
        {
            Removed = SortByName(removed),
            Added = SortByName(added),
            Updated = updated.OrderBy(change => change.Name, NameComparer).ToList(),
            HasRecord = true,
            RecordedAt = record.RecordedAt,
            GameVersion = record.GameVersion,
            RecordedModCount = record.Mods.Count,
            CurrentModCount = currentMods.Count
        };
    }

    /// <summary>按显示名排序（三类列表各自排序，本次比对内部排序一致）。</summary>
    /// <param name="names">模组显示名。</param>
    /// <returns>排序后的显示名。</returns>
    private static IReadOnlyList<string> SortByName(IEnumerable<string> names)
    {
        return names.OrderBy(name => name, NameComparer).ToList();
    }
}
