using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

public class TextureManager
{
    public static TextureManager Instance { get; } = new();

    private readonly Dictionary<MenuTabId, Texture2D> textures  = new();
    public Texture2D FarmTexture => this.textures[MenuTabId.Farm];
    public Texture2D TownTexture => this.textures[MenuTabId.Town];
    public Texture2D MountainTexture => this.textures[MenuTabId.Mountain];
    public Texture2D ForestTexture => this.textures[MenuTabId.Forest];
    public Texture2D BeachTexture => this.textures[MenuTabId.Beach];
    public Texture2D DesertTexture => this.textures[MenuTabId.Desert];
    public Texture2D GingerIslandTexture => this.textures[MenuTabId.GingerIsland];
    public Texture2D SVETexture => this.textures[MenuTabId.SVE];
    public Texture2D RSVTexture => this.textures[MenuTabId.RSV];

    public void LoadTexture(IModHelper helper)
    {
        this.textures.Add(MenuTabId.Farm, helper.ModContent.Load<Texture2D>("assets/Farm.png"));
        this.textures.Add(MenuTabId.Town, helper.ModContent.Load<Texture2D>("assets/Town.png"));
        this.textures.Add(MenuTabId.Mountain, helper.ModContent.Load<Texture2D>("assets/Mountain.png"));
        this.textures.Add(MenuTabId.Forest, helper.ModContent.Load<Texture2D>("assets/Forest.png"));
        this.textures.Add(MenuTabId.Beach, helper.ModContent.Load<Texture2D>("assets/Beach.png"));
        this.textures.Add(MenuTabId.Desert, helper.ModContent.Load<Texture2D>("assets/Desert"));
        this.textures.Add(MenuTabId.GingerIsland, helper.ModContent.Load<Texture2D>("assets/GingerIsland.png"));
        this.textures.Add(MenuTabId.RSV, helper.ModContent.Load<Texture2D>("assets/RSV.png"));
        this.textures.Add(MenuTabId.SVE, helper.ModContent.Load<Texture2D>("assets/SVE.png"));
    }
}