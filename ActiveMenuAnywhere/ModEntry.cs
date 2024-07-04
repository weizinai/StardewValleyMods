using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    public static readonly Dictionary<MenuTabId, Texture2D> Textures = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        this.LoadTexture();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new Game1Patcher(this.config, helper));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForActiveMenuAnywhere(
            new GenericModConfigMenuIntegration<ModConfig>(
                this.Helper.ModRegistry,
                this.ModManifest,
                () => this.config,
                () =>
                {
                    this.config = new ModConfig();
                    this.Helper.WriteConfig(this.config);
                },
                () => this.Helper.WriteConfig(this.config)
            )
        ).Register();
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.config.OpenMenuByTelephone) return;

        if (this.config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else if (Context.IsPlayerFree)
                Game1.activeClickableMenu = new AMAMenu(this.config.DefaultMeanTabId, this.config, this.Helper);
        }
    }

    private void LoadTexture()
    {
        Textures.Add(MenuTabId.Farm, this.Helper.ModContent.Load<Texture2D>("Assets/Farm.png"));
        Textures.Add(MenuTabId.Town, this.Helper.ModContent.Load<Texture2D>("Assets/Town.png"));
        Textures.Add(MenuTabId.Mountain, this.Helper.ModContent.Load<Texture2D>("Assets/Mountain.png"));
        Textures.Add(MenuTabId.Forest, this.Helper.ModContent.Load<Texture2D>("Assets/Forest.png"));
        Textures.Add(MenuTabId.Beach, this.Helper.ModContent.Load<Texture2D>("Assets/Beach.png"));
        Textures.Add(MenuTabId.Desert, this.Helper.ModContent.Load<Texture2D>("Assets/Desert"));
        Textures.Add(MenuTabId.GingerIsland, this.Helper.ModContent.Load<Texture2D>("Assets/GingerIsland.png"));
        Textures.Add(MenuTabId.RSV, this.Helper.ModContent.Load<Texture2D>("Assets/RSV.png"));
        Textures.Add(MenuTabId.SVE, this.Helper.ModContent.Load<Texture2D>("Assets/SVE.png"));
    }
}