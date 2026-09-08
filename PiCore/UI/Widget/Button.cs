using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 按钮：九宫格底 + 单行文本，可悬停（宿主鼠标路由控制 <see cref="Hovered" />）并可点击（<see cref="OnClick" />）。
/// 同时是焦点图的可获焦项：<see cref="Element.Focusable" /> 为 true，<see cref="Element.ActivateAction" /> 兼作
/// 手柄 A 的激活（与鼠标左键同一动作，一次按下只触发一次）。获焦时绘制金色描边环（控制器导航指示）。
/// 本文件为已验证原型（UiFrameworkProto）Button 的移植（含悬停视觉 + 获焦视觉）。
/// </summary>
public class Button : Element
{
    /// <summary>文本左右内边距（按钮最小宽度 = 文本宽 + 2×该值）。</summary>
    private const float HorizontalPadding = 48f;

    /// <summary>文本上下内边距（按钮最小高度 = 文本高 + 2×该值）。</summary>
    private const float VerticalPadding = 24f;

    /// <summary>悬停描边相对按钮边框的外扩量（像素，描边宽 = 按钮宽 + 2×该值）。</summary>
    private const int HoverRingOutset = 3;

    /// <summary>获焦描边外扩量（像素，比悬停光晕略大：同时获焦+悬停时金色实环仍露在光晕外侧）。</summary>
    private const int FocusRingOutset = 5;

    private string text;

    /// <summary>构造按钮。</summary>
    /// <param name="text">按钮文本。</param>
    public Button(string text)
    {
        this.text = text;
    }

    /// <summary>按钮文本（改动后标脏重排）。</summary>
    public string Text
    {
        get => this.text;
        set
        {
            this.text = value;
            this.MarkDirty();
        }
    }

    /// <summary>点击回调（宿主把左键命中路由到最上层按钮后触发；手柄 A 经 <see cref="Element.ActivateAction" /> 兼用同一动作）。</summary>
    public Action? OnClick { get; set; }

    /// <summary>按钮始终可获焦（焦点图收集；获焦视觉见 <see cref="DrawSelf" /> 的金色描边环）。</summary>
    public override bool Focusable => true;

    /// <summary>手柄 A 的激活动作 = 鼠标点击动作（一次按下只触发一条路径，无双触发）。</summary>
    public override Action? ActivateAction => this.OnClick;

    /// <summary>鼠标悬停状态：由宿主的悬停路由置位/复位（<c>internal set</c> 只允许框架内部改，消费模组可读）。</summary>
    public bool Hovered { get; internal set; }

    /// <summary>盒子底色：常态白、悬停浅黄。派生控件可覆写以表达选中态等额外状态。</summary>
    protected virtual Color BoxColor => this.Hovered ? Color.LightYellow : Color.White;

    /// <summary>盒子透明度（如选中态用半透明区分；1 = 不透明）。</summary>
    protected virtual float BoxOpacity => 1f;

    /// <summary>文本颜色（随 <see cref="BoxColor" /> 可读性调整：金底等亮色底用深色文字）。</summary>
    protected virtual Color TextColor => this.Hovered ? Color.Black : Theme.TextColor;

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        var size = Theme.SmallFont.MeasureString(this.text);
        var width = Math.Min(size.X + HorizontalPadding * 2, available.X);
        var height = Math.Min(size.Y + VerticalPadding * 2, available.Y);

        return new Vector2(width, height);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        // 就地布置：文本水平垂直居中于本元素
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        if (this.IsFocused)
        {
            // 获焦：金色实心描边环（外扩 FocusRingOutset，按钮底覆盖内芯后呈一圈金边；比悬停光晕更实，指示控制器焦点）
            Theme.DrawPanel(batch, this.Bounds.X - FocusRingOutset, this.Bounds.Y - FocusRingOutset, this.Bounds.Width + FocusRingOutset * 2,
                this.Bounds.Height + FocusRingOutset * 2, Color.Gold);
        }

        if (this.Hovered)
        {
            // 悬停：外圈高亮描边 + 浅黄底 + 深字（SDV 原生悬停反馈的扁平化呈现）
            Theme.DrawPanel(batch, this.Bounds.X - HoverRingOutset, this.Bounds.Y - HoverRingOutset, this.Bounds.Width + HoverRingOutset * 2,
                this.Bounds.Height + HoverRingOutset * 2, Color.Gold * 0.5f);
        }

        Theme.DrawPanel(batch, this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height, this.BoxColor * this.BoxOpacity);

        var textSize = Theme.SmallFont.MeasureString(this.text);
        Theme.DrawText(
            batch,
            this.text,
            new Vector2(this.Bounds.Center.X - textSize.X / 2f, this.Bounds.Center.Y - textSize.Y / 2f),
            this.TextColor);
    }
}
