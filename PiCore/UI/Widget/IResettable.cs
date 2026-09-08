namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 需要在“重新显示/激活”时回到初始状态的视图内容（票 06 引入的框架缝，供 <see cref="TabControl" />
/// 在切换页签时重置内容状态）。<see cref="Pager" /> 实现它：重置即回到第一页。
/// 消费方自建的可重置内容也可实现本接口，让页签切换“回到第一页/初始态”自然生效。
/// 是否实现由内容自己决定：需要保留状态（如长列表滚动位置）的内容不应实现本接口。
/// </summary>
public interface IResettable
{
    /// <summary>把内容重置到初始状态（如分页回到第一页）。</summary>
    public void Reset();
}
