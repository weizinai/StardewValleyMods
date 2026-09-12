using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.SaveModInfo.Record;

namespace weizinai.StardewValleyMod.SaveModInfo.UI;

/// <summary>
/// 叠层根视图：把活动菜单的槽位几何映射成每槽的图块，持「当前打开的是哪个存档」，
/// 绘制跟随光标的悬停概览并挂载模态窗口。
/// </summary>
/// <remarks>
///     <para>
///     本视图**铺满视口**而非按内容自适应：压暗层与点击拦截层都必须是全屏的。图块因此走
///     <see cref="Canvas" /> 的绝对定位（偏移即屏幕坐标）。
///     </para>
///     <para>
///     子级只有两层，加入顺序即绘制顺序：图块在最底，模态窗口在其上（窗口的拦截层随之压住全部图块）。
///     悬停概览**不进布局树**——它由本视图在绘制期按当前光标现测现摆现画（与框架自带提示框同一设计，
///     见 <c>MenuHost.DrawTooltip</c>），因此光标移动不会牵动任何重排。
///     </para>
///     <para>
///     显示与否完全由调用方经 <see cref="SyncSlots" /> / <see cref="SyncHover" /> 逐帧写入，本视图不做任何
///     菜单身份判断（那是叠层宿主的职责）。图块与窗口都是布局树内的节点，写入一律按「值真正变化才写」：
///     它们的 setter 会标脏重排。
///     </para>
///     <para>
///     槽位只按唯一键重建图块：滚动列表换来换去的是同一批存档时复用既有图块，悬停态与点击目标都不会被打断。
///     </para>
///     <para>
///     窗口内容跟着差异结果走：差异结果换了实例（写入新记录会作废缓存，重新进入存档页会重建缓存）时就地重建
///     窗口内容，因此打开的窗口不会停在上一次比对的结果上。缓存里的差异结果按存档名复用同一实例，稳态下这条
///     刷新路径每帧都是空转，不影响窗口内的滚动位置。
///     </para>
/// </remarks>
internal sealed class ModInfoOverlayRoot : Element
{
    /// <summary>一个槽位的图块与它对应的存档键（点击时要凭键找回那个存档的差异结果）。</summary>
    /// <param name="Key">存档文件夹名。</param>
    /// <param name="Tile">该槽的图块。</param>
    private sealed record SlotEntry(string Key, SaveSlotTile Tile);

    private readonly Canvas tiles = new();

    /// <summary>悬停概览面板；**不在布局树里**（浮动层，与框架自带提示框同一设计），由本视图在绘制期现测现摆现画。</summary>
    private readonly ModInfoHoverPanel hover = new();

    private readonly ModInfoWindow window;

    /// <summary>当前每槽的「存档键 + 图块」（一个槽一份，键与图块不拆成两个平行列表）。</summary>
    private readonly List<SlotEntry> slotEntries = new();

    /// <summary>本帧悬停图块的差异结果；null = 不显示概览（没有悬停任何图块，或模态窗口正打开）。</summary>
    private ModDiff? hoverDiff;

    private Point hoverCursor;

    /// <summary>打开的窗口对应哪个存档；null = 窗口没开。</summary>
    private string? windowKey;

    /// <summary>窗口内容上次据以重建的差异结果（与当前实例不同即重建，避免窗口显示陈旧数据）。</summary>
    private ModDiff? windowDiff;

    /// <summary>构造根视图。</summary>
    /// <param name="onCloseRequest">模态窗口请求关闭时执行的动作（关闭按钮与 <c>Esc</c> 共用）。</param>
    public ModInfoOverlayRoot(Action onCloseRequest)
    {
        this.window = new ModInfoWindow(onCloseRequest)
        {
            // 模态窗口靠可见性手动启停：叠层没有「菜单打开/关闭」的生命周期可依托
            Visible = false
        };

        // 顺序即绘制顺序：图块在最底，模态窗口在其上（拦截层随之压住全部图块）
        this.Add(this.tiles);
        this.Add(this.window);
    }

    /// <summary>模态窗口当前是否打开（叠层宿主据此决定是否拦截 <c>Esc</c> 与吞掉左键）。</summary>
    public bool IsWindowOpen => this.window.Visible;

    /// <summary>按本帧的槽位几何与差异结果同步图块（键序列变化时重建，否则复用并按值更新）。</summary>
    /// <param name="slots">本帧每个可见存档槽的图标数据。</param>
    public void SyncSlots(IReadOnlyList<SaveSlotIcon> slots)
    {
        if (!this.HasSameKeys(slots))
        {
            this.RebuildTiles(slots);
        }

        for (var i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            var tile = this.slotEntries[i].Tile;

            // 仅在位置真正变化时写入：Canvas 的偏移表 setter 会标脏整条父链，每帧无条件写会让布局每帧重跑
            if (this.tiles.GetChildPosition(tile) != slot.Position)
            {
                this.tiles.SetChildPosition(tile, slot.Position);
            }

            tile.Sync(slot.Diff);
        }

        this.RefreshOpenWindow();
    }

    /// <summary>按本帧的悬停状态同步概览面板（光标位置只在绘制期用，故这里只记下来）。</summary>
    /// <param name="cursorX">光标 X（UI 坐标）。</param>
    /// <param name="cursorY">光标 Y（UI 坐标）。</param>
    public void SyncHover(int cursorX, int cursorY)
    {
        this.hoverCursor = new Point(cursorX, cursorY);

        // 窗口打开时图块被拦截层压住，概览不该露在压暗层上
        var diff = this.window.Visible ? null : this.FindHoveredTile()?.Diff;

        if (ReferenceEquals(this.hoverDiff, diff))
        {
            return;
        }

        this.hoverDiff = diff;
        this.hover.Sync(diff);
    }

    /// <summary>打开模态窗口并列出该存档的明细。</summary>
    /// <param name="key">存档键（图块持有的键）。</param>
    /// <returns>是否成功打开（找不到图块或该槽还没有差异结果时为 false，窗口保持原状）。</returns>
    public bool OpenWindow(string key)
    {
        var diff = this.FindTile(key)?.Diff;

        if (diff is null)
        {
            return false;
        }

        this.windowKey = key;
        this.SyncWindow(diff);
        this.SetWindowVisible(true);

        return true;
    }

    /// <summary>关闭模态窗口（关闭按钮与 <c>Esc</c> 都走这里）。</summary>
    public void CloseWindow()
    {
        this.windowKey = null;
        this.windowDiff = null;
        this.SetWindowVisible(false);
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch batch)
    {
        base.Draw(batch);

        // 悬停概览是浮动层（不进布局树）：绘制在所有子级之上，按当前光标现测现摆。窗口打开时它必须消失，
        // 否则会露在压暗层上继续跟随光标
        if (this.hoverDiff is null || this.window.Visible)
        {
            return;
        }

        // Place 依赖 DesiredSize，故必须先测量（与框架提示框的绘制期流程同一顺序）
        this.hover.Measure(new Vector2(Game1.uiViewport.Width, Game1.uiViewport.Height));

        var size = this.hover.DesiredSize;
        var position = this.hover.Place(this.hoverCursor.X, this.hoverCursor.Y);

        this.hover.Arrange(new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y));
        this.hover.Draw(batch);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        // 两层都无条件测量：窗口可能正隐藏（未布置时读不到尺寸，显示的那一帧就会画在零矩形上）。
        // 悬停概览不进布局树，由绘制期现测现摆，故不在此处测量
        this.tiles.Measure(available);
        this.window.Measure(available);

        // 铺满视口：本视图是叠层，绝不按内容自适应（压暗层与拦截层必须全屏）
        return new Vector2(available.X, available.Y);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        this.tiles.Arrange(final);

        if (this.window.Visible)
        {
            this.window.Arrange(final);
        }
    }

    /// <summary>同步模态窗口的可见性（从隐藏转显示时标脏，让拦截层与面板拿到真实矩形）。</summary>
    /// <param name="visible">是否显示。</param>
    private void SetWindowVisible(bool visible)
    {
        if (this.window.Visible == visible)
        {
            return;
        }

        this.window.Visible = visible;

        // <see cref="Element.Visible" /> 的 setter 不标脏，而容器按可见性跳过测量/布置：
        // 不标脏的话刚露出的窗口会停在零矩形，既不画也不命中
        this.MarkDirty();
    }

    /// <summary>把窗口内容同步到当前的差异结果（打开窗口时与差异结果换实例时各调用一次）。</summary>
    /// <param name="diff">该存档最新的差异结果。</param>
    private void SyncWindow(ModDiff diff)
    {
        this.windowDiff = diff;
        this.window.Sync(diff);
    }

    /// <summary>
    /// 窗口打开期间差异结果换了实例时重建窗口内容。
    /// </summary>
    /// <remarks>
    /// 差异结果按存档名缓存并复用同一实例，因此这里平常每帧都是空转；真正会变的只有「写入新记录」与
    /// 「重新进入存档页」两处缓存失效之后。图块滚出这一屏时找不到键，此时保持现状：窗口内容不该因为滚动而清空。
    /// </remarks>
    private void RefreshOpenWindow()
    {
        if (this.windowKey is null)
        {
            return;
        }

        var diff = this.FindTile(this.windowKey)?.Diff;

        if (diff is null || ReferenceEquals(diff, this.windowDiff))
        {
            return;
        }

        this.SyncWindow(diff);
    }

    /// <summary>按槽位键重建全部图块（键序列变化时调用）。</summary>
    /// <param name="slots">本帧每个可见存档槽的图标数据。</param>
    private void RebuildTiles(IReadOnlyList<SaveSlotIcon> slots)
    {
        this.tiles.Clear();
        this.slotEntries.Clear();

        foreach (var slot in slots)
        {
            // 按键闭包捕获：图块点击时打开的必须是它自己那个存档
            var key = slot.Key;
            var tile = new SaveSlotTile(() => this.OpenWindow(key));

            this.slotEntries.Add(new SlotEntry(key, tile));
            this.tiles.SetChildPosition(tile, slot.Position);
            this.tiles.Add(tile);
        }
    }

    /// <summary>本帧槽位键序列与当前图块是否完全一致（一致即复用图块，避免每次滚动都重建）。</summary>
    /// <param name="slots">本帧每个可见存档槽的图标数据。</param>
    /// <returns>数量与顺序都一致时为 true。</returns>
    private bool HasSameKeys(IReadOnlyList<SaveSlotIcon> slots)
    {
        if (this.slotEntries.Count != slots.Count)
        {
            return false;
        }

        for (var i = 0; i < slots.Count; i++)
        {
            if (!string.Equals(this.slotEntries[i].Key, slots[i].Key, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>按存档键找图块。</summary>
    /// <param name="key">存档键。</param>
    /// <returns>图块；没有该键时为 null。</returns>
    private SaveSlotTile? FindTile(string key)
    {
        foreach (var entry in this.slotEntries)
        {
            if (string.Equals(entry.Key, key, StringComparison.Ordinal))
            {
                return entry.Tile;
            }
        }

        return null;
    }

    /// <summary>当前被鼠标悬停的图块（同时只有一个）。</summary>
    /// <returns>图块；没有悬停任何图块时为 null。</returns>
    private SaveSlotTile? FindHoveredTile()
    {
        foreach (var entry in this.slotEntries)
        {
            // 一并判可见：隐藏的图块（与记录一致、没有变化）不该还挂着悬停态
            if (entry.Tile.Visible && entry.Tile.IsHovered)
            {
                return entry.Tile;
            }
        }

        return null;
    }
}
