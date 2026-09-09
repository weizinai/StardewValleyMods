using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

/// <summary>
/// 收藏页签内容：每次被选中（<see cref="TabControl" /> 激活本内容）时依据当前的
/// <see cref="ModConfig.FavoriteMenus" /> 重建分页内容，等价于旧版「切到收藏页签重建菜单」的行为。
/// 收藏列表在菜单打开期间会因收藏/取消收藏而变化，因此必须每次重建而非复用静态内容。
/// </summary>
internal class FavoriteTabContent : Element, IResettable
{
    private Pager pager = null!;

    /// <summary>构造并立即按当前收藏列表构建内容。</summary>
    public FavoriteTabContent()
    {
        this.Rebuild();
    }

    /// <summary>实现 <see cref="IResettable" />：页签被选中时按当前收藏列表重建。</summary>
    public void Reset()
    {
        this.Rebuild();
    }

    /// <summary>依据当前收藏选项重建分页内容。</summary>
    private void Rebuild()
    {
        if (this.pager is not null)
        {
            this.Remove(this.pager);
        }

        this.pager = PagerBuilder.Build(OptionFactory.CreateFavoriteOptions(), isFavoriteTab: true);
        this.Add(this.pager);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        this.pager.Measure(available);

        return this.pager.DesiredSize;
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        this.pager.Arrange(final);
    }
}
