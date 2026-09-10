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
/// 纯视图与输入适配：开关语义（含「未手持晶球不许开」的守卫）由本模组的会话（<c>AutoBreakSession</c>）定义，构造时以
/// <c>toggleAutoBreak</c> 注入，本类只负责把它接到按钮上。
/// 单实例复用：本模组只建一枚按钮，靠每帧的 <see cref="Sync" /> 按活动菜单身份幂等启停，避免订阅残留与跨菜单残留绘制。
/// 显示与否只由活动菜单身份决定（晶球菜单在即显示，无配置开关）；文案与锚位也由同一次 <see cref="Sync" /> 现取——
/// 水平取面板左缘（<c>IClickableMenu.xPositionOnScreen</c>）、垂直取晶球区顶边（<c>GeodeMenu.geodeSpot.bounds.Y</c>），
/// 窗口尺寸变化后原版会重新居中面板并重建晶球点击区，每帧读取天然跟随。故「先文案、再冲刷测量、最后取锚位」
/// 这条顺序不再由调用方承担（见 <see cref="Sync" /> 的 remarks）。
/// 输入由模组侧显式驱动，全程无 Harmony 补丁：悬停路由随每帧的 <see cref="Sync" /> 一并发起；
/// 左键由 SMAPI 输入事件交给 <see cref="HandleLeftClick" /> 命中并触发点击，命中后调用方**吞掉**这一击。
/// 吞掉是必需而非可选：<see cref="DrawableHost" /> 会把出视口的位置夹回视口，视口过窄（晶球面板宽 880 UI 像素
/// = 800 + 边框 40×2，左侧放不下按钮加间隙）时按钮会被夹到面板左缘内侧、横向落进晶球点击区，
/// 不吞就会一次点击同时翻转开关并砸开一颗晶球。
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
        // 宿主「创建即订阅并开始绘制」，故建完立刻隐藏：可见性由每帧的 Sync 按活动菜单身份决定
        this.drawableHost.Disable();
    }

    /// <summary>把叠层同步到本帧的事实：按活动菜单身份启停、同步文案、路由悬停、按面板几何取锚位。</summary>
    /// <remarks>
    /// 内部顺序有意固定，且不外露给调用方：文案必须先在测量之前落地（锚位按新文案的实测宽度右对齐），
    /// 测量必须先于取锚位、且只能借悬停路由顺带冲刷挂载布局——宿主的输入方法在禁用时直接返回、不冲刷，
    /// 故启用必须排在悬停之前。悬停不按窗口焦点分流：失焦时 SMAPI 不再刷新输入状态，读到的仍是失焦前的光标，
    /// 路由结果自然停在原处，与原版「失焦不派发悬停」的可见效果一致。
    /// </remarks>
    /// <param name="geodeMenu">当前活动菜单；不是晶球菜单（无菜单或换成别的菜单）时给 <c>null</c>，叠层随即隐藏。</param>
    /// <param name="autoBreak">当前是否正在自动砸（决定按钮文案）。</param>
    public void Sync(GeodeMenu? geodeMenu, bool autoBreak)
    {
        this.SyncText(autoBreak);

        if (geodeMenu is null)
        {
            // 菜单不在（或被其它菜单取代）即隐藏：既不残留绘制与命中，也不在菜单外继续绘制
            this.drawableHost.Disable();

            return;
        }

        this.drawableHost.Enable();
        this.drawableHost.PerformHoverAction(Game1.getMouseX(), Game1.getMouseY());
        this.AnchorGeodeButton(geodeMenu.xPositionOnScreen, geodeMenu.geodeSpot.bounds.Y);
    }

    /// <summary>把左键交给叠层做命中与点击。</summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>
    /// true = 命中并已触发点击；false = 未命中或叠层未显示。返回值供调用方据此**吞掉这一击**（见 <see cref="DrawableHost.HandleLeftClick" /> 的独占消费契约）：
    /// 锚位在窗口过窄时会被宿主夹回面板内侧，按钮落进晶球点击区，不吞就会一次点击同时翻转开关并砸开一颗晶球。
    /// </returns>
    public bool HandleLeftClick(int x, int y)
    {
        return this.drawableHost.HandleLeftClick(x, y);
    }

    /// <summary>
    /// 把按钮贴到面板左缘外侧、与晶球区顶边齐平：右缘与面板左缘留 <see cref="PanelGap" /> 间隙。
    /// 宽度取按钮的实测宽度，故文案变长（「停止」或其它语言）时按钮只向左生长，与面板的间隙恒定。
    /// </summary>
    /// <param name="panelLeft">面板左缘（<c>IClickableMenu.xPositionOnScreen</c>）。</param>
    /// <param name="geodeSpotTop">晶球区顶边（<c>GeodeMenu.geodeSpot.bounds.Y</c>）。</param>
    private void AnchorGeodeButton(float panelLeft, float geodeSpotTop)
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
    private void SyncText(bool autoBreak)
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
}
