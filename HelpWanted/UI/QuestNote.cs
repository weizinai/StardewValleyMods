using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using weizinai.StardewValleyMod.HelpWanted.Model;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.HelpWanted.UI;

/// <summary>
/// 板面上一张钉着的便签：位置由任务管理器摆放好（<see cref="PlacedBounds" />），外观是“纸底 → 图钉 → 头像”三层贴图，
/// 顺序不能反（头像要盖在纸底上）。
/// 交互（悬停 / 左键 / 手柄 A / 金色焦点环）由内嵌的全格透明 <see cref="Button" /> 承担——AMA <c>OptionTile</c>
/// 的已验证姿势；绘制顺序也是先子级（环）再自身美术，于是焦点环只在便签边缘露出一圈，不遮住便签本体。
/// </summary>
public class QuestNote : Element
{
    /// <summary>透明按钮：盒体不绘制（透明度 0），只保留悬停/点击/手柄焦点环视觉。</summary>
    private sealed class TransparentButton : Button
    {
        public TransparentButton()
            : base(string.Empty)
        {
        }

        /// <summary>盒体完全透明，便签美术由外层绘制。</summary>
        protected override float BoxOpacity => 0f;
    }

    private readonly TransparentButton button;

    /// <summary>便签的贴图与配色数据（含它代表的任务）。</summary>
    public QuestModel QuestModel { get; }

    /// <summary>管理器算好的摆放矩形（屏幕绝对坐标），视图按它布置本便签。</summary>
    public Rectangle PlacedBounds { get; }

    /// <summary>便签被点击 / 手柄 A 激活时的动作（由视图接线到宿主）。</summary>
    public Action? OnActivate
    {
        get => this.button.OnClick;
        set => this.button.OnClick = value;
    }

    /// <summary>构造便签。</summary>
    /// <param name="questModel">便签的贴图与配色数据。</param>
    /// <param name="bounds">摆放矩形（屏幕绝对坐标）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="questModel"/> 为 <see langword="null"/>。</exception>
    public QuestNote(QuestModel questModel, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(questModel);

        this.QuestModel = questModel;
        this.PlacedBounds = bounds;
        this.button = new TransparentButton();
        this.Add(this.button);
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch batch)
    {
        if (!this.Visible)
        {
            return;
        }

        // 先画子级：透明按钮的悬停/焦点环内芯会被随后的便签美术盖住，只在便签边缘露出一圈
        foreach (var child in this.Children)
        {
            if (child.Visible)
            {
                child.Draw(batch);
            }
        }

        var bounds = this.Bounds;
        batch.Draw(this.QuestModel.Pad, bounds, this.QuestModel.PadSource, this.QuestModel.PadColor);
        batch.Draw(this.QuestModel.Pin, bounds, this.QuestModel.PinSource, this.QuestModel.PinColor);
        batch.Draw(
            this.QuestModel.Icon,
            new Vector2(bounds.X + this.QuestModel.IconOffset.X, bounds.Y + this.QuestModel.IconOffset.Y),
            this.QuestModel.IconSource,
            this.QuestModel.IconColor,
            0,
            Vector2.Zero,
            this.QuestModel.IconScale,
            SpriteEffects.None,
            0
        );
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        return new Vector2(this.PlacedBounds.Width, this.PlacedBounds.Height);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        this.button.Arrange(final);
    }
}
