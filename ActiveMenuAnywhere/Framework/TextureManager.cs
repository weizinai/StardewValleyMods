using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

public class TextureManager
{
    public static TextureManager Instance { get; } = new();

    private Dictionary<MenuTabId, Texture2D> Textures { get; } = new();
    public Texture2D FarmTexture => this.Textures[MenuTabId.Farm];
    public Texture2D TownTexture => this.Textures[MenuTabId.Town];
    public Texture2D MountainTexture => this.Textures[MenuTabId.Mountain];
    public Texture2D ForestTexture => this.Textures[MenuTabId.Forest];
    public Texture2D BeachTexture => this.Textures[MenuTabId.Beach];
    public Texture2D DesertTexture => this.Textures[MenuTabId.Desert];
    public Texture2D GingerIslandTexture => this.Textures[MenuTabId.GingerIsland];
    public Texture2D SVETexture => this.Textures[MenuTabId.SVE];
    public Texture2D RSVTexture => this.Textures[MenuTabId.RSV];

    public void LoadTexture(IModHelper helper)
    {
        this.Textures.Add(MenuTabId.Farm, helper.ModContent.Load<Texture2D>("assets/Farm.png"));
        this.Textures.Add(MenuTabId.Town, helper.ModContent.Load<Texture2D>("assets/Town.png"));
        this.Textures.Add(MenuTabId.Mountain, helper.ModContent.Load<Texture2D>("assets/Mountain.png"));
        this.Textures.Add(MenuTabId.Forest, helper.ModContent.Load<Texture2D>("assets/Forest.png"));
        this.Textures.Add(MenuTabId.Beach, helper.ModContent.Load<Texture2D>("assets/Beach.png"));
        this.Textures.Add(MenuTabId.Desert, helper.ModContent.Load<Texture2D>("assets/Desert"));
        this.Textures.Add(MenuTabId.GingerIsland, helper.ModContent.Load<Texture2D>("assets/GingerIsland.png"));
        this.Textures.Add(MenuTabId.RSV, helper.ModContent.Load<Texture2D>("assets/RSV.png"));
        this.Textures.Add(MenuTabId.SVE, helper.ModContent.Load<Texture2D>("assets/SVE.png"));
    }
}