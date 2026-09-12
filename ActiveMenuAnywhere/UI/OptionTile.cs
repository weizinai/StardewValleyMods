using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Config;
using weizinai.StardewValleyMod.PiCore.Hud;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.UI;

/// <summary>
/// 选项格子：一个元素即一个可交互的菜单单元，占满网格单元。
/// 交互（悬停/点击/手柄焦点环）由全格透明的 <see cref="Button" /> 子元素承担，
/// 视觉（位置艺术图 + 扁平标签胶囊）由本元素自身的绘制层绘制。
/// 点击 / 手柄 A 激活：复刻旧版自绘菜单的选项点击分支——按住收藏键时
/// 收藏/取消收藏，否则按启用条件 <see cref="BaseOption.Apply" /> 或弹「暂不可用」提示。
/// 表现数据（标签/图标/身份）来自 <see cref="OptionEntry" />（目录行物化），
/// 行为（<see cref="BaseOption.IsEnable" />/<see cref="BaseOption.Apply" />）来自其内嵌的行为实例。
/// </summary>
internal class OptionTile : Element
{
    /// <summary>透明按钮：盒体不绘制（透明度 0），仅保留悬停/点击/手柄焦点环视觉。</summary>
    private sealed class TransparentButton : Button
    {
        public TransparentButton()
            : base(string.Empty)
        {
        }

        /// <summary>盒体完全透明，艺术图与标签由外部绘制层呈现。</summary>
        protected override float BoxOpacity => 0f;
    }

    /// <summary>格子尺寸（与网格单元一致，720p 视口下完整可见）。</summary>
    internal const float CellSize = 168f;

    /// <summary>
    /// 环内缩：艺术图比格子四周各内缩该值，让透明按钮的焦点/悬停环（面板从格子边缘
    /// 外扩 5px）在格子内部露出一圈——网格单元边对边紧排，环外扩部分会被相邻格子覆盖，
    /// 只有把环留在格子内部才能在每一个格子上看到完整焦点环。
    /// </summary>
    private const float RingInset = 6f;

    /// <summary>艺术图源区域边长（每张位置图为 200×200 的单元）。</summary>
    private const int ArtSize = 200;

    /// <summary>标签胶囊相对格子顶部的位置（原版在 200 格内位于 y+120，按格子高度等比缩放）。</summary>
    private const float LabelOffsetRatio = 0.6f;

    /// <summary>标签胶囊横向内边距。</summary>
    private const int PillPaddingX = 16;

    /// <summary>标签胶囊纵向内边距。</summary>
    private const int PillPaddingY = 8;

    private readonly OptionEntry entry;
    private readonly bool isFavoriteTab;

    /// <summary>透明全格按钮：承载悬停/点击/手柄焦点环，绘制层在其上铺艺术图与标签。</summary>
    private readonly TransparentButton button;

    /// <summary>构造选项格子：行为 + 表现数据双输入（表现数据来自目录行物化，不再从行为实例读取）。</summary>
    /// <param name="entry">目录行物化出的条目（标签/图标/身份 + 行为实例）。</param>
    /// <param name="isFavoriteTab">是否收藏页签（决定收藏键点击分支）。</param>
    public OptionTile(OptionEntry entry, bool isFavoriteTab)
    {
        this.entry = entry;
        this.isFavoriteTab = isFavoriteTab;
        this.button = new TransparentButton
        {
            OnClick = this.OnActivate
        };
        this.Add(this.button);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        return new Vector2(CellSize, CellSize);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        this.button.Arrange(final);
    }

    /// <summary>
    /// 绘制顺序：先让透明按钮画悬停/焦点环（金色面板铺满格子+外扩），
    /// 再在环之上铺艺术图与标签——环只在格子边缘露一圈，不遮住艺术图本体。
    /// </summary>
    /// <param name="batch">精灵批。</param>
    public override void Draw(SpriteBatch batch)
    {
        if (!this.Visible)
        {
            return;
        }

        foreach (var child in this.Children)
        {
            if (child.Visible)
            {
                child.Draw(batch);
            }
        }

        this.DrawArt(batch);
    }

    /// <summary>绘制艺术图（居中铺满格子）与标签胶囊（扁平九宫格 + 扁平文字）。</summary>
    /// <param name="batch">精灵批。</param>
    private void DrawArt(SpriteBatch batch)
    {
        // 艺术图：把 200×200 源区域等比缩放到「格子减去两侧环内缩」的尺寸，居中绘制。
        // 环内缩在格子边缘留出金色环的可见带，相邻格子覆盖不到。
        var center = this.Bounds.Center;
        var scale = (CellSize - RingInset * 2) / ArtSize;
        batch.Draw(
            this.entry.Texture,
            new Vector2(center.X, center.Y),
            this.entry.SourceRect,
            Color.White,
            0f,
            new Vector2(ArtSize / 2f, ArtSize / 2f),
            scale,
            SpriteEffects.None,
            0f
        );

        // 标签胶囊：九宫格底 + 扁平文字，水平居中、垂直位于格子 60% 处
        var textSize = Theme.SmallFont.MeasureString(this.entry.Label);
        var pillWidth = (int)textSize.X + PillPaddingX * 2;
        var pillHeight = (int)textSize.Y + PillPaddingY * 2;
        var pillX = center.X - pillWidth / 2;
        var pillY = this.Bounds.Y + (int)(this.Bounds.Height * LabelOffsetRatio);

        Theme.DrawPanel(batch, pillX, pillY, pillWidth, pillHeight);
        Theme.DrawText(batch, this.entry.Label, new Vector2(pillX + PillPaddingX, pillY + PillPaddingY));
    }

    /// <summary>激活分支（鼠标左键 / 手柄 A）：还原旧版选项点击逻辑。</summary>
    private void OnActivate()
    {
        if (ModConfig.Instance.FavoriteKey.IsDown())
        {
            this.HandleFavoriteKey();
        }
        else if (this.entry.Option.IsEnable() || !ModConfig.Instance.ProgressMode)
        {
            this.entry.Option.Apply();
        }
        else
        {
            Game1.drawObjectDialogue(I18n.UI_Tip_Unavailable());
        }
    }

    /// <summary>按住收藏键时的分支：收藏页签内点击取消收藏，其余页签点击加入/提示已存在。</summary>
    private void HandleFavoriteKey()
    {
        if (this.isFavoriteTab)
        {
            ModConfig.Instance.FavoriteMenus.Remove(this.entry.Id);
            HudLogger.NoIconHUDMessage(I18n.UI_Favorite_Remove(), 1000);
        }
        else if (!ModConfig.Instance.FavoriteMenus.Contains(this.entry.Id))
        {
            ModConfig.Instance.FavoriteMenus.Add(this.entry.Id);
            HudLogger.NoIconHUDMessage(I18n.UI_Favorite_Add(), 1000);
        }
        else
        {
            HudLogger.NoIconHUDMessage(I18n.UI_Favorite_Exist(), 1000);
        }
    }
}