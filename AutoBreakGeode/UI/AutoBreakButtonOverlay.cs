using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.UI.Host;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.AutoBreakGeode.UI;

/// <summary>
/// 晶球菜单的开始/停止按钮叠层：把一枚 PiCore UI <see cref="Button" /> 经交互式叠层宿主
/// <see cref="DrawableHost" /> 叠在活动菜单之上，锚在**面板左缘外侧、与晶球区顶边齐平**处。
/// 常规视口下（左缘外放得下按钮）与 560×308 的晶球点击区完全不相交；视口过窄时锚位会被宿主夹进面板，
/// 届时依赖调用方吞掉命中的点击（见本类文档的 remarks 段）。
/// </summary>
/// <remarks>
/// 纯视图与输入适配：开关语义（含「未手持晶球不许开」的守卫）由本模组的状态持有者定义，构造时以
/// <c>toggleAutoBreak</c> 注入，本类只负责把它接到按钮上。
/// 单实例复用：本模组只建一枚按钮，靠 <see cref="IsShown" /> 按活动菜单身份幂等启停，避免订阅残留与跨菜单残留绘制。
/// 锚位由模组侧每帧给出：水平取面板左缘（<c>IClickableMenu.xPositionOnScreen</c>），垂直取晶球区顶边
/// （<c>GeodeMenu.geodeSpot.bounds.Y</c>）——窗口尺寸变化后原版会重新居中面板并重建晶球点击区，每帧读取天然跟随；
/// 显示与否只由活动菜单身份决定（晶球菜单在即显示，无配置开关）。
/// 输入由模组侧显式驱动，全程无 Harmony 补丁：悬停由模组在 <c>UpdateTicked</c> 里交给 <see cref="PerformHoverAction" />；
/// 左键由 SMAPI 输入事件交给 <see cref="HandleLeftClick" /> 命中并触发点击，命中后调用方**吞掉**这一击。
/// 吞掉是必需而非可选：<see cref="DrawableHost" /> 会把出视口的位置夹回视口，视口过窄（面板宽 832 UI 像素，
/// 装不下按钮加间隙）时按钮会被夹到面板左缘内侧、横向落进晶球点击区，不吞就会一次点击同时翻转开关并砸开一颗晶球。
/// 叠层画在活动菜单（含其末尾手持物）之后，会盖住光标旁的物品图；根视图在按钮之后按原版坐标补画手持物，
/// 宿主再把光标补到最上层，z 序与原版一致（按钮 → 手持物 → 光标）。
/// 观感与音效（悬停金环 + 浅黄底、悬停/确认音）全部取自框架扁平默认。
/// </remarks>
internal sealed class AutoBreakButtonOverlay
{
    /// <summary>
    /// 叠层根：尺寸跟随按钮，绘制时先画按钮再补画原版手持物。补画必须走根视图而不是另订
    /// <c>RenderedActiveMenu</c>——宿主 <c>Disable</c>/<c>Enable</c> 会退订再订阅，后订的外部处理器会跑到宿主前面，
    /// 手持物又会被按钮盖住。
    /// </summary>
    private sealed class HeldItemOverlayRoot : Element
    {
        private readonly Button toggleButton;

        public HeldItemOverlayRoot(Button toggleButton)
        {
            this.toggleButton = toggleButton;
            this.Add(toggleButton);
        }

        /// <inheritdoc />
        protected override Vector2 MeasureOverride(Vector2 available)
        {
            return this.toggleButton.Measure(available);
        }

        /// <inheritdoc />
        protected override void ArrangeOverride(Rectangle final)
        {
            this.toggleButton.Arrange(final);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch batch)
        {
            if (!this.Visible)
            {
                return;
            }

            base.Draw(batch);

            if (Game1.activeClickableMenu is not MenuWithInventory { heldItem: { } heldItem })
            {
                return;
            }

            // 坐标与原版 GeodeMenu.draw 同源（getOldMouseX/Y + 8），叠在按钮之上、光标之下
            heldItem.drawInMenu(batch, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
        }
    }

    /// <summary>按钮右缘与面板左缘之间的间隙。</summary>
    private const int PanelGap = 8;

    private readonly Button toggleButton;
    private readonly DrawableHost drawableHost;

    /// <summary>叠层当前是否显示（set 幂等：false 时连残留悬停态一并复位）。</summary>
    public bool IsShown
    {
        get => this.drawableHost.IsEnabled;
        set
        {
            if (value)
            {
                this.drawableHost.Enable();
            }
            else
            {
                this.drawableHost.Disable();
            }
        }
    }

    /// <summary>构造叠层并订阅活动菜单渲染事件；随即回到隐藏态（只在晶球菜单打开时才可见）。</summary>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="toggleAutoBreak">点击按钮时执行的开关切换（语义与守卫由调用方定义）。</param>
    public AutoBreakButtonOverlay(IDisplayEvents display, Action toggleAutoBreak)
    {
        // 这里的文案只是首帧占位，不作缓存：构造发生在游戏应用已保存的语言之前（见 SyncText），真实文案由每帧同步写入
        this.toggleButton = new Button(I18n.UI_BeginButton())
        {
            OnClick = toggleAutoBreak
        };
        this.drawableHost = DrawableHost.CreateDrawable(new HeldItemOverlayRoot(this.toggleButton), display, DrawableHost.RenderSlot.ActiveMenu, Vector2.Zero);
        // 宿主「创建即订阅并开始绘制」，故建完立刻隐藏：可见性由 AutoBreakHandler 每帧按活动菜单身份决定
        this.drawableHost.Disable();
    }

    /// <summary>
    /// 把按钮贴到面板左缘外侧、与晶球区顶边齐平：右缘与面板左缘留 <see cref="PanelGap" /> 间隙。
    /// 宽度取按钮的实测宽度，故文案变长（「停止」或其它语言）时按钮只向左生长，与面板的间隙恒定。
    /// </summary>
    /// <param name="panelLeft">面板左缘（<c>IClickableMenu.xPositionOnScreen</c>）。</param>
    /// <param name="geodeSpotTop">晶球区顶边（<c>GeodeMenu.geodeSpot.bounds.Y</c>）。</param>
    public void AnchorOutsidePanel(float panelLeft, float geodeSpotTop)
    {
        var position = new Vector2(panelLeft - PanelGap - this.GetButtonWidth(panelLeft), geodeSpotTop);

        // 仅在实际变化时写入：Position 的 setter 会标脏根视图，每帧无条件写会让布局每帧重跑（绘制抖动）
        if (this.drawableHost.Position != position)
        {
            this.drawableHost.Position = position;
        }
    }

    /// <summary>
    /// 取按钮宽度：优先用排布出的实测值，尚未排布时（首帧 / 本帧刚改过文案）就地测量补上。
    /// 就地测量取按钮自己的实测宽度而非固定兜底值——文字随语言变长时兜底值会偏窄，按钮右缘就会压进面板。
    /// </summary>
    /// <param name="panelLeft">面板左缘（UI 像素），用作测量时可用的水平空间。</param>
    /// <returns>按钮宽度（与 <see cref="PanelGap" /> 同为 UI 像素）。</returns>
    private float GetButtonWidth(float panelLeft)
    {
        return this.toggleButton.Bounds.Width > 0
            ? this.toggleButton.Bounds.Width
            : this.toggleButton.Measure(new Vector2(panelLeft, Game1.uiViewport.Height)).X;
    }

    /// <summary>按自动砸标志同步按钮文字：关态「开始」、开态「停止」，两种文案均按当前语言现取。</summary>
    /// <param name="autoBreak">当前是否正在自动砸。</param>
    public void SyncText(bool autoBreak)
    {
        // 文案每帧现取而不缓存：模组的 Entry 早于游戏应用已保存的语言（SMAPI 在模组加载后才把翻译切到该语言），
        // 构造期取一次会把英文基文案永久留在按钮上——中文下按钮恒显示 Begin/Stop。每帧的代价只是一次字典查找
        var text = autoBreak ? I18n.UI_StopButton() : I18n.UI_BeginButton();

        // 仅在实际变化时写入：Button.Text 的 setter 会标脏重排，每帧无条件写会让布局每帧重跑（绘制抖动）
        if (!string.Equals(this.toggleButton.Text, text, StringComparison.Ordinal))
        {
            this.toggleButton.Text = text;
        }
    }

    /// <summary>把光标位置交给叠层做悬停路由（置/复位最上层按钮的悬停态与悬停音效）。</summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    public void PerformHoverAction(int x, int y)
    {
        this.drawableHost.PerformHoverAction(x, y);
    }

    /// <summary>把左键交给叠层做命中与点击。</summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>
    /// true = 命中并已触发点击；false = 未命中或叠层未显示。返回值供调用方据此**吞掉这一击**（见 <see cref="DrawableHost.HandleLeftClick" /> 的独占消费契约）。
    /// </returns>
    public bool HandleLeftClick(int x, int y)
    {
        return this.drawableHost.HandleLeftClick(x, y);
    }
}
