using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.SaveModInfo.Record;

/// <summary>
/// 存档模组信息的存取边界：唯一读写记录文件、唯一创建「当前模组快照」的地方。对外三件事：取某个存档
/// 相对记录的差异、为某个存档写入当前快照、以及在「新的存档页实例出现」时丢弃缓存。
/// </summary>
/// <remarks>
///     <para>
///     记录文件路径不变（模组数据目录下的 <c>data/&lt;存档文件夹名&gt;.json</c>，键即存档页菜单槽上的
///     <c>Farmer.slotName</c>），形状升级为自描述对象（记录时间 + 游戏版本 + 每个模组唯一 ID → 名称与版本）。
///     <b>旧形状</b>（平字典 <c>{唯一ID: 名称}</c>）不兼容读取：JSON 反序列化未启用「未知成员报错」，
///     旧文件喂给新模型会静默解析成模组表为空的记录，于是「未记录」的判定天然覆盖旧形状——模组表为空即视为
///     没记录过，玩家载入并保存一次后自动写成新形状。
///     </para>
///     <para>
///     判定「未记录」的三条路径：文件不存在与解析失败（损坏的 JSON，记一条日志）在本类收敛到 <see cref="Read" />
///     返回的 null，读到空 <see cref="ModInfoRecord.Mods" />（旧形状落地形态）则由 <see cref="RecordDiff.Compare" />
///     判为「没有记录过」——规则落在纯函数上，本类不替它归一。
///     读取是**按需**的（存档页要画哪个槽才读哪个），因此不再有「启动时扫一遍存档目录」这一步，存档目录不存在
///     或为空时既不需要报错也没有可报的错。
///     </para>
///     <para>
///     记录、当前模组快照与算好的差异都带缓存：叠层每帧都要取每个可见槽的差异，每帧重读文件、重建设置表并重算
///     会白烧 CPU。三处缓存一起在 <see cref="Write" />（写入新记录）与 <see cref="ClearCache" />（新的存档页实例
///     出现）失效，避免窗口显示陈旧数据。
///     </para>
/// </remarks>
internal sealed class RecordStore
{
    /// <summary>记录文件夹在模组数据目录下的相对路径片段。</summary>
    private const string DataFolder = "data";

    private readonly IModHelper helper;
    private readonly IModRegistry modRegistry;

    /// <summary>按存档文件夹名缓存的记录；null = 文件不存在或解析失败（旧形状的空模组表由差异纯函数判为「没有记录过」）。</summary>
    private readonly Dictionary<string, ModInfoRecord?> recordCache = new();

    /// <summary>按存档文件夹名缓存的差异结果（叠层每帧都要取，故连结果一起缓存，避免每帧重算）。</summary>
    private readonly Dictionary<string, ModDiff> diffCache = new();

    /// <summary>当前已加载模组的快照（懒建：模组在模组加载期才陆续登记完，本类构造时还拿不全）。</summary>
    private IReadOnlyDictionary<string, ModInfoEntry>? currentMods;

    /// <summary>构造存取边界。</summary>
    /// <param name="helper">消费模组的 helper（数据目录与读写入口）。</param>
    public RecordStore(IModHelper helper)
    {
        this.helper = helper;
        this.modRegistry = helper.ModRegistry;
    }

    /// <summary>取某个存档相对记录的差异；没有记录（见类注释的三条路径）时返回标注「无记录」的空差异。</summary>
    /// <param name="saveFolderName">存档文件夹名。</param>
    /// <returns>差异结果（含元信息与「有无记录」标志）。</returns>
    public ModDiff GetDiff(string saveFolderName)
    {
        if (this.diffCache.TryGetValue(saveFolderName, out var cached))
        {
            return cached;
        }

        var diff = RecordDiff.Compare(this.Read(saveFolderName), this.GetCurrentMods());

        this.diffCache[saveFolderName] = diff;

        return diff;
    }

    /// <summary>为某个存档写入当前快照（记录时间取当前现实时间，游戏版本取当前运行版本）。</summary>
    /// <param name="saveFolderName">存档文件夹名。</param>
    public void Write(string saveFolderName)
    {
        var record = new ModInfoRecord
        {
            // 记录时间只到分钟：这个字段的作用是让玩家判断「这份对比有多新」，秒级精度没有意义
            RecordedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            GameVersion = Game1.version,
            Mods = this.GetCurrentMods().ToDictionary(entry => entry.Key, entry => entry.Value)
        };

        this.helper.Data.WriteJsonFile($"{DataFolder}/{saveFolderName}.json", record);
        this.diffCache.Clear();
        this.recordCache[saveFolderName] = record;
        Logger<ModEntry>.Info(I18n.UI_Record_Log());
    }

    /// <summary>丢弃全部缓存的记录与差异（新的存档页实例出现时调用，避免显示上一次实例留下的陈旧数据）。</summary>
    public void ClearCache()
    {
        this.recordCache.Clear();
        this.diffCache.Clear();
    }

    /// <summary>取当前已加载模组的快照（首次调用时建立并缓存；模组集合在模组加载期之后就固定了）。</summary>
    /// <returns>当前已加载模组：唯一 ID → 名称与版本。</returns>
    private IReadOnlyDictionary<string, ModInfoEntry> GetCurrentMods()
    {
        if (this.currentMods is not null)
        {
            return this.currentMods;
        }

        var mods = new Dictionary<string, ModInfoEntry>();

        foreach (var mod in this.modRegistry.GetAll())
        {
            mods[mod.Manifest.UniqueID] = new ModInfoEntry(mod.Manifest.Name, mod.Manifest.Version.ToString());
        }

        this.currentMods = mods;

        return mods;
    }

    /// <summary>
    /// 读某个存档的记录（带缓存）：文件不存在与解析失败返回 null；旧形状（平字典）解析出的空模组表原样返回，
    /// 由 <see cref="RecordDiff.Compare" /> 判为「没有记录过」。
    /// </summary>
    /// <param name="saveFolderName">存档文件夹名。</param>
    /// <returns>记录；文件不存在或解析失败时为 null。</returns>
    private ModInfoRecord? Read(string saveFolderName)
    {
        if (this.recordCache.TryGetValue(saveFolderName, out var cached))
        {
            return cached;
        }

        ModInfoRecord? record;

        try
        {
            record = this.helper.Data.ReadJsonFile<ModInfoRecord>($"{DataFolder}/{saveFolderName}.json");
        }
        catch (Exception ex)
        {
            // 损坏的 JSON 无法解析：按「没有记录过」处理，但要留下日志供排查（玩家载入并保存一次即可修复）
            Logger<ModEntry>.Error($"Failed to read the mod information record for '{saveFolderName}': {ex.Message}");
            record = null;
        }

        this.recordCache[saveFolderName] = record;

        return record;
    }
}
