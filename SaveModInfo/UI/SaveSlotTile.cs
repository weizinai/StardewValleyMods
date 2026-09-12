using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;
using weizinai.StardewValleyMod.SaveModInfo.Record;

namespace weizinai.StardewValleyMod.SaveModInfo.UI;

/// <summary>
/// 单个存档槽的提示图块：自绘改造前那枚原版感叹号 sprite，并内嵌一个透明按钮承担悬停、点击与吞击。
/// </summary>
/// <remarks>
///     <para>
///     <b>几何与改造前逐像素一致</b>：图块左上角 = 槽位左上角 + <c>(128 + 36 + 存档名文本宽, 36 - 4)</c>，
///     命中区 44×48，sprite 取 <c>mouseCursors</c> 上 <c>(383, 495, 11, 12)</c> 放大 4 倍——三者都沿用改造前那套
///     Harmony 补丁里的原值，界面换成叠层绘制后图标的位置与命中手感不变。
///     </para>
///     <para>
///     图块自身不决定显示与否（由根视图按差异结果设置 <see cref="Element.Visible" />）；本类只负责画与命中。
///     透明按钮（<see cref="IconButton.BoxOpacity" /> = 0）只为拿到框架的悬停/点击路由：按钮是可获焦项、点击即消费，
///     于是点图标不会同时把这一击下发给原版存档槽（否则会直接开始载入存档）。
///     </para>
/// </remarks>
internal sealed class SaveSlotTile : Element
{
    /// <summary>槽位左上角到图标左上角的水平偏移里「不随存档名变化」的部分（与改造前的补丁同源）。</summary>
    private const int IconOffsetX = 128 + 36;

    /// <summary>槽位左上角到图标左上角的垂直偏移（与改造前的补丁同源）。</summary>
    private const int IconOffsetY = 36 - 4;

    /// <summary>命中区宽度（与改造前的补丁同源）。</summary>
    private const int HitWidth = 44;

    /// <summary>命中区高度（与改造前的补丁同源）。</summary>
    private const int HitHeight = 48;

    /// <summary>感叹号 sprite 在 <c>Game1.mouseCursors</c> 上的源矩形（与改造前的补丁同源）。</summary>
    private static readonly Rectangle IconSourceRect = new(383, 495, 11, 12);

    /// <summary>sprite 放大倍数（与改造前的补丁同源）。</summary>
    private const float IconScale = 4f;

    /// <summary>只带交互、不画盒体的按钮（悬停时仍画框架的悬停描边，指示可点）。</summary>
    private sealed class IconButton : Button
    {
        /// <summary>构造空文案的透明按钮。</summary>
        public IconButton() : base(string.Empty) { }

        /// <summary>盒体完全透明：视觉交给图块自绘的 sprite，按钮只留交互。</summary>
        protected override float BoxOpacity => 0f;
    }

    private readonly IconButton button;

    /// <summary>上次写入的差异结果，用于判断内容有没有真的变化（避免每帧重排）。</summary>
    private ModDiff? diff;

    /// <summary>构造图块。</summary>
    /// <param name="onClick">点击图标时执行的动作（打开模态窗口）。</param>
    public SaveSlotTile(Action onClick)
    {
        this.button = new IconButton { OnClick = onClick };
        this.Add(this.button);
    }

    /// <summary>本槽的差异结果（由根视图写入，供悬停概览与模态窗口取用）；未同步过时为 null。</summary>
    public ModDiff? Diff => this.diff;

    /// <summary>图块是否正被鼠标悬停（悬停概览面板据此显示）。</summary>
    public bool IsHovered => this.button.Hovered;

    /// <summary>
    /// 按差异结果同步图块：三类变化里任一有内容、或从未记录过时显示图标；与记录完全一致的存档不显示。
    /// </summary>
    /// <param name="value">该存档的差异结果。</param>
    public void Sync(ModDiff value)
    {
        if (ReferenceEquals(this.diff, value))
        {
            return;
        }

        this.diff = value;

        // 判据是「三类里任一有内容」，不再是改造前那句「有已移除」：只加过模组、或只有模组升过版本的存档
        // 同样是值得提醒的变化。从未记录过的存档也显示图标，否则玩家会把「没记录」误当成「一切一致」
        var shouldShow = value.HasChanges || !value.HasRecord;

        if (this.Visible != shouldShow)
        {
            this.Visible = shouldShow;

            // Visible 的 setter 不标脏（框架不通知），而容器按可见性跳过测量/布置：不改可见性时新露出的图块
            // 会停在「从未布置」（零矩形）状态，既画不出也命中不了
            this.MarkDirty();
        }
    }

    /// <summary>按槽位左上角算出图块左上角（水平偏移含按存档名文本宽度计算的量）。</summary>
    /// <param name="slotBounds">原版槽位矩形（<c>LoadGameMenu.slotButtons[i].bounds</c>）。</param>
    /// <param name="farmerName">原版画在槽位里的那行名字（<c>SaveFileSlot.slotName()</c> 返回的 <c>Farmer.Name</c>）。</param>
    /// <returns>图块左上角在 UI 坐标的位置。</returns>
    public static Vector2 GetPosition(Rectangle slotBounds, string farmerName)
    {
        return new Vector2(
            slotBounds.X + IconOffsetX + SpriteText.getWidthOfString(farmerName),
            slotBounds.Y + IconOffsetY);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        return new Vector2(HitWidth, HitHeight);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        this.button.Arrange(final);
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        // 原版图标画在最底层；悬停描边由内嵌按钮画在其上（描边在 44×48 之外一圈，不盖住 sprite）
        batch.Draw(
            Game1.mouseCursors,
            new Vector2(this.Bounds.X, this.Bounds.Y),
            IconSourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            IconScale,
            SpriteEffects.None,
            0f
        );
    }
}
