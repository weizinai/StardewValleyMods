using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

internal class TextureManager
{
    private const string PadTexturePath = "aedenthorn.HelpWanted/pad";
    private const string PinTexturePath = "aedenthorn.HelpWanted/pin";

    private IModHelper helper = null!;

    private Texture2D defaultPadTexture = null!;
    private Texture2D defaultPinTexture = null!;
    private readonly Dictionary<string, Texture2D> textureCache = new();

    public static readonly TextureManager Instance = new();

    public void Init(IModHelper _helper)
    {
        this.helper = _helper;
        this.defaultPadTexture = _helper.ModContent.Load<Texture2D>("assets/Pad.png");
        this.defaultPinTexture = _helper.ModContent.Load<Texture2D>("assets/Pin.png");
    }

    public Texture2D GetPinTexture(string targetNPC, string questType)
    {
        return this.GetTexture(PinTexturePath, targetNPC, questType) ?? this.defaultPinTexture;
    }

    public Texture2D GetPadTexture(string targetNPC, string questType)
    {
        return this.GetTexture(PadTexturePath, targetNPC, questType) ?? this.defaultPadTexture;
    }

    private Texture2D? GetTexture(string basePath, string targetNPC, string questType)
    {
        var pathsToCheck = new[]
        {
            $"{basePath}/{targetNPC}/{questType}",
            $"{basePath}/{targetNPC}",
            $"{basePath}/{questType}",
            basePath
        };

        foreach (var path in pathsToCheck)
        {
            var variantTextures = this.GetTextureVariants(path);
            if (variantTextures.Count > 0)
            {
                return Game1.random.ChooseFrom(variantTextures);
            }

            try
            {
                if (!this.textureCache.ContainsKey(path))
                {
                    var texture = this.helper.GameContent.Load<Texture2D>(path);
                    this.textureCache.Add(path, texture);
                }
                return this.textureCache[path];
            }
            catch
            {
                // ignored
            }
        }

        return null;
    }

    private List<Texture2D> GetTextureVariants(string basePath)
    {
        var textures = new List<Texture2D>();
        var i = 1;

        while (true)
        {
            try
            {
                var path = $"{basePath}/{i}";
                if (!this.textureCache.ContainsKey(path))
                {
                    var texture = this.helper.GameContent.Load<Texture2D>(path);
                    this.textureCache.Add(path, texture);
                }
                textures.Add(this.textureCache[path]);
                i++;
            }
            catch
            {
                break;
            }
        }

        return textures;
    }

    public Color GetRandomColor()
    {
        var random = ModEntry.Random;
        var config = ModConfig.Instance;

        return new Color(
            (byte)random.Next(config.RandomColorMin, config.RandomColorMax),
            (byte)random.Next(config.RandomColorMin, config.RandomColorMax),
            (byte)random.Next(config.RandomColorMin, config.RandomColorMax)
        );
    }
}