using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Config;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

/// <summary>
/// 物化后的选项条目：把目录行的表现数据（身份/标签/图标）与工厂构造的行为实例合成为格子可直接消费的形态。
/// 收藏增删用 <see cref="Id" />，激活走 <see cref="Option" />，绘制读 <see cref="Label" />/<see cref="Texture" />/<see cref="SourceRect" />。
/// </summary>
/// <param name="Id">选项身份（字符串 id，与 <see cref="ModConfig.FavoriteMenus" /> 一致）。</param>
/// <param name="Label">物化时解析好的本地化标签。</param>
/// <param name="Texture">所属页签的图标纹理表。</param>
/// <param name="SourceRect">图标序号在纹理表中的源区域。</param>
/// <param name="Option">工厂构造的行为实例。</param>
internal sealed record OptionEntry(
    string Id,
    string Label,
    Texture2D Texture,
    Rectangle SourceRect,
    BaseOption Option
);
