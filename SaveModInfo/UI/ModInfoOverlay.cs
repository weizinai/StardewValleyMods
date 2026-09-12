using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.UI.Host;
using weizinai.StardewValleyMod.SaveModInfo.Record;
using static StardewValley.Menus.LoadGameMenu;

namespace weizinai.StardewValleyMod.SaveModInfo.UI;

/// <summary>
/// 叠层宿主与同步：持有叠层宿主，按当前菜单身份幂等启停，写入每槽的图块数据，路由悬停、左键与滚轮。
/// </summary>
/// <remarks>
///     <para>
///     <b>门控条件是「标题菜单当前的子菜单是原版加载存档页」，不能用活动菜单判断</b>：「加载存档」页不是
///     <c>Game1.activeClickableMenu</c>，而是标题菜单的子菜单（<c>TitleMenu.subMenu</c>），活动菜单是
///     <c>TitleMenu</c> 本身。也正因如此本模组不把界面做成独立菜单——那需要接管标题菜单并在每条关闭路径上
///     把它放回去，任何一条漏掉都会让玩家停在无菜单的标题画面。
///     </para>
///     <para>
///     类型必须是原版那一类（<c>GetType() == typeof(LoadGameMenu)</c>）：联机选档（<c>CoopMenu</c>）与选角色
///     （<c>FarmhandMenu</c>）都是它的子类，那些界面的槽是 <c>HostFileSlot</c> / <c>FarmhandSlot</c>、语义不同，
///     本模组的图标与窗口一律不出现在那里（与原版补丁时期的判断一致）。
///     </para>
///     <para>
///     每帧同步的内部顺序有意固定：<b>先启用宿主、再同步内容、最后路由悬停</b>。宿主的输入方法在禁用时直接返回
///     且不冲刷布局，而悬停命中依赖已排布的矩形——顺序颠倒会按上一帧（甚至零矩形）的矩形命中。
///     </para>
///     <para>
///     输入由模组侧显式驱动，全程无 Harmony 补丁：左键与滚轮由 SMAPI 输入事件转给宿主，模态窗口打开期间两者都被
///     吞掉（窗口是模态的，背后的存档槽不该收到任何点击或滚动）；<c>Esc</c> 只在窗口打开时拦截，窗口没开时放行
///     原版行为（原版存档页的 <c>Esc</c> = 回标题菜单）。
///     </para>
/// </remarks>
internal sealed class ModInfoOverlay
{
    private readonly RecordStore store;
    private readonly ModInfoOverlayRoot root;
    private readonly DrawableHost host;

    /// <summary>本帧要画的图块数据（复用同一实例，避免每帧分配）。</summary>
    private readonly List<SaveSlotIcon> slots = new();

    /// <summary>上一次同步时的存档页实例（用于识别「换了新的存档页」并作废残留状态）。</summary>
    private LoadGameMenu? trackedMenu;

    /// <summary>构造叠层并订阅活动菜单渲染事件；随即回到隐藏态（只在原版存档页显示时才可见）。</summary>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="store">记录与差异的存取边界。</param>
    public ModInfoOverlay(IDisplayEvents display, RecordStore store)
    {
        this.store = store;
        this.root = new ModInfoOverlayRoot(this.CloseWindow);
        this.host = DrawableHost.CreateDrawable(this.root, display, DrawableHost.RenderSlot.ActiveMenu, Vector2.Zero);

        // 宿主「创建即订阅并开始绘制」，故建完立刻隐藏：可见性完全由每帧的 Sync 按菜单身份决定
        this.host.Disable();
    }

    /// <summary>模态窗口当前是否打开（调用方据此决定是否拦截 <c>Esc</c> 与吞掉左键、滚轮）。</summary>
    public bool IsWindowOpen => this.root.IsWindowOpen;

    /// <summary>把叠层同步到本帧的事实：按菜单身份与状态启停、重建每槽的图块数据、路由悬停。</summary>
    public void Sync()
    {
        var menu = GetVanillaLoadGameMenu();

        if (menu is null || !IsShowable(menu))
        {
            this.Hide();

            return;
        }

        // 新的存档页实例：记录缓存与窗口状态都随实例作废，免得窗口显示上一次实例留下的陈旧数据
        if (!ReferenceEquals(this.trackedMenu, menu))
        {
            this.trackedMenu = menu;
            this.root.CloseWindow();
            this.store.ClearCache();
        }

        this.host.Enable();
        this.BuildSlots(menu);
        this.root.SyncSlots(this.slots);

        // 悬停路由排在内容同步之后：宿主在命中的同时冲刷挂起布局，先路由会按上一帧的矩形命中
        var mouseX = Game1.getMouseX();
        var mouseY = Game1.getMouseY();

        this.host.PerformHoverAction(mouseX, mouseY);
        this.root.SyncHover(mouseX, mouseY);
    }

    /// <summary>把左键交给叠层做命中与点击。</summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>
    /// true = 命中叠层按钮并已触发点击（调用方据此**吞掉**这一击）。图标的命中区落在原版存档槽内部，
    /// 不吞的话点图标会同时开始载入存档；模态窗口打开时拦截层铺满视口，因此一切左键都在此被消费。
    /// </returns>
    public bool HandleLeftClick(int x, int y)
    {
        return this.host.HandleLeftClick(x, y);
    }

    /// <summary>把滚轮增量交给叠层，滚动光标下最上层的列表（模态窗口打开时即窗口内的明细列表）。</summary>
    /// <param name="delta">SMAPI 滚轮事件的增量（vanilla 符号）。</param>
    public void HandleScrollWheel(int delta)
    {
        this.host.PerformScrollAction(delta);
    }

    /// <summary>关闭模态窗口（<c>Esc</c> 与窗口内的关闭按钮都走这里）。</summary>
    public void CloseWindow()
    {
        this.root.CloseWindow();
    }

    /// <summary>隐藏整个叠层（菜单不在、或当前状态不该显示时调用；幂等）。</summary>
    private void Hide()
    {
        // 先关窗再禁用宿主：禁用后输入方法一律不生效，残留的窗口会停在那里画不出也关不掉
        this.root.CloseWindow();
        this.host.Disable();
    }

    /// <summary>按当前存档页的槽位几何与差异结果重建本帧的图块数据。</summary>
    /// <param name="menu">原版加载存档页。</param>
    private void BuildSlots(LoadGameMenu menu)
    {
        this.slots.Clear();

        for (var i = 0; i < menu.slotButtons.Count; i++)
        {
            var index = menu.currentItemIndex + i;

            // 不足一屏时末尾的空槽不画：原版也不画它们的名字，图标没有可依附的位置
            if (index >= menu.MenuSlots.Count)
            {
                continue;
            }

            if (menu.MenuSlots[index] is not SaveFileSlot slot)
            {
                continue;
            }

            // 处于「游戏版本不匹配」的槽位不画我们的图标：原版在该槽画自己的版本提示，两个提示叠起来会互相干扰
            if (slot.versionComparison < 0)
            {
                continue;
            }

            var farmer = slot.Farmer;

            this.slots.Add(new SaveSlotIcon(
                farmer.slotName,
                SaveSlotTile.GetPosition(menu.slotButtons[i].bounds, farmer.Name),
                this.store.GetDiff(farmer.slotName)));
        }
    }

    /// <summary>当前显示的原版加载存档页；不是它时返回 null（叠层随之整块隐藏）。</summary>
    /// <returns>原版加载存档页；不在该界面时为 null。</returns>
    private static LoadGameMenu? GetVanillaLoadGameMenu()
    {
        if (Game1.activeClickableMenu is not TitleMenu)
        {
            return null;
        }

        // 子类（联机选档 / 选角色）不算：它们的槽位语义不同，本模组的界面不该出现在那里
        return TitleMenu.subMenu is LoadGameMenu menu && menu.GetType() == typeof(LoadGameMenu) ? menu : null;
    }

    /// <summary>当前状态是否该显示叠层（与原版行为对齐，避免在过渡状态里留下越界提示）。</summary>
    /// <param name="menu">原版加载存档页。</param>
    /// <returns>该显示时为 true。</returns>
    /// <remarks>
    /// <c>IsDoingTask()</c> 一并覆盖「列表尚未异步就绪」「删除中」「载入淡出中」三种过渡状态；
    /// 删除确认界面是独立状态位，需单独判。
    /// </remarks>
    private static bool IsShowable(LoadGameMenu menu)
    {
        return !menu.deleteConfirmationScreen && !menu.IsDoingTask();
    }
}
