using System.Collections.Generic;

namespace weizinai.StardewValleyMod.SaveModInfo.Record;

/// <summary>
/// 某个存档的模组信息记录（记录文件的 JSON 形状，自描述）：记录时间、记录时的游戏版本、以及每个已加载模组的名称与版本。
/// </summary>
/// <remarks>
/// <b>写入与读取都只能经 <see cref="RecordStore" /></b>：本类型只是文件形状，不含任何存取逻辑。
/// 全部成员是公共自动属性（可序列化），默认构造 + 属性赋值即可被 JSON 反序列化填满；旧形状的平字典
/// 喂进来会得到一个 <see cref="Mods" /> 为空的实例（未启用未知成员报错），<see cref="RecordDiff.Compare" />
/// 把这个空表判为「没有记录过」。
/// </remarks>
internal class ModInfoRecord
{
    /// <summary>记录写下的时间（<c>yyyy-MM-dd HH:mm</c>，本地时间）。</summary>
    public string RecordedAt { get; set; } = string.Empty;

    /// <summary>记录时的游戏版本（<c>Game1.version</c>）。</summary>
    public string GameVersion { get; set; } = string.Empty;

    /// <summary>记录时已加载的模组：唯一 ID → 名称与版本。</summary>
    public Dictionary<string, ModInfoEntry> Mods { get; set; } = new();
}
