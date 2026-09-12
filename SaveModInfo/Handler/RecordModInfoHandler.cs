using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.SaveModInfo.Record;

namespace weizinai.StardewValleyMod.SaveModInfo.Handler;

/// <summary>
/// 记录时机：**完全不变**——载入时若为每月 1 号则立即记录，否则本会话首次保存时记录。
/// </summary>
/// <remarks>
/// 只负责「什么时候记」，怎么记与记成什么形状都在 <see cref="RecordStore" /> 里（记录内容已升级为自描述形状：
/// 记录时间、游戏版本、每个模组的名称与版本）。
/// </remarks>
internal class RecordModInfoHandler : BaseHandler
{
    private readonly RecordStore store;

    /// <summary>构造处理器。</summary>
    /// <param name="helper">消费模组的 helper。</param>
    /// <param name="store">记录与差异的存取边界（与叠层处理器共用同一实例）。</param>
    public RecordModInfoHandler(IModHelper helper, RecordStore store) : base(helper)
    {
        this.store = store;
    }

    /// <inheritdoc />
    public override void Apply()
    {
        this.helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    /// <summary>载入存档：每月 1 号立即记录，其余日期挂一次「本会话首次保存」的钩子。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据（未用）。</param>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.dayOfMonth == 1)
        {
            this.RecordModInfo();
        }
        else
        {
            this.helper.Events.GameLoop.Saved += this.OnSaved;
        }
    }

    /// <summary>本会话首次保存：记录一次并立刻卸钩（本会话不再重复记录）。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据（未用）。</param>
    private void OnSaved(object? sender, SavedEventArgs e)
    {
        this.RecordModInfo();

        this.helper.Events.GameLoop.Saved -= this.OnSaved;
    }

    /// <summary>把当前模组快照写进当前存档的记录文件。</summary>
    private void RecordModInfo()
    {
        // 存档文件夹名在存档上下文不可用时为 null（正常记录路径上必然可用，此处只作兜底，避免写进 data/.json）
        var saveFolderName = Constants.SaveFolderName;

        if (saveFolderName is null)
        {
            Logger<ModEntry>.Error("The current save folder name is unavailable; the mod information was not recorded.");

            return;
        }

        this.store.Write(saveFolderName);
    }
}
