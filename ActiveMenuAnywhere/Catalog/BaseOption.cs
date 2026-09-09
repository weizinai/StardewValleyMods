using weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

/// <summary>
/// 选项行为基类：只留启用判定与应用动作两个行为口。
/// 标签/图标/纹理等表现数据由 <see cref="OptionCatalog" /> 目录行承载、经物化层解析，
/// 不再落在选项类上；构造不再读取 <see cref="TextureManager" /> 或 I18n，可免游戏构造。
/// </summary>
internal abstract class BaseOption
{
    /// <summary>按当前游戏进度判定该选项是否可用。</summary>
    public virtual bool IsEnable()
    {
        return true;
    }

    /// <summary>应用动作：打开该选项对应的菜单。</summary>
    public abstract void Apply();
}
