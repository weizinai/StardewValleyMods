using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;

/// <summary>
/// 页签 → 纹理表：各页签的图标纹理在目录物化时按页签字符串 id 取用，
/// 不再由选项类各自持有。加载时机仍为 <see cref="LoadTexture" />（游戏内）。
/// 键与 <see cref="OptionCatalog" /> 的页签 id 词表共用常量（除 Favorite 无纹理表外，
/// 每个位置页签一张纹理）。纹理资源路径与页签的对应关系在此显式声明，改动页签资源时同步这里。
/// </summary>
internal class TextureManager
{
    public static TextureManager Instance { get; } = new();

    private readonly Dictionary<string, Texture2D> textures = new();

    public void LoadTexture(IModHelper helper)
    {
        this.textures.Add(OptionCatalog.FarmTabId, helper.ModContent.Load<Texture2D>("assets/Farm.png"));
        this.textures.Add(OptionCatalog.TownTabId, helper.ModContent.Load<Texture2D>("assets/Town.png"));
        this.textures.Add(OptionCatalog.MountainTabId, helper.ModContent.Load<Texture2D>("assets/Mountain.png"));
        this.textures.Add(OptionCatalog.ForestTabId, helper.ModContent.Load<Texture2D>("assets/Forest.png"));
        this.textures.Add(OptionCatalog.BeachTabId, helper.ModContent.Load<Texture2D>("assets/Beach.png"));
        this.textures.Add(OptionCatalog.DesertTabId, helper.ModContent.Load<Texture2D>("assets/Desert.png"));
        this.textures.Add(OptionCatalog.GingerIslandTabId, helper.ModContent.Load<Texture2D>("assets/GingerIsland.png"));
        this.textures.Add(OptionCatalog.RsvTabId, helper.ModContent.Load<Texture2D>("assets/RSV.png"));
        this.textures.Add(OptionCatalog.SveTabId, helper.ModContent.Load<Texture2D>("assets/SVE.png"));
    }

    /// <summary>按页签 id 取图标纹理表（Favorite 页签无纹理表，勿查询）。</summary>
    /// <param name="tabId">页签 id。</param>
    /// <returns>该页签的纹理表。</returns>
    public Texture2D GetTexture(string tabId)
    {
        return this.textures[tabId];
    }
}
