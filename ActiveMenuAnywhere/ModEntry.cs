using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        ModConfig.Init(helper);
        OptionFactory.Init(helper);
        TextureManager.Instance.LoadTexture(helper);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new Game1Patcher(helper));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForActiveMenuAnywhere(
            new GenericModConfigMenuIntegration<ModConfig>(
                this.Helper.ModRegistry,
                this.ModManifest,
                () => ModConfig.Instance,
                () =>
                {
                    ModConfig.Instance = new ModConfig();
                    this.Helper.WriteConfig(ModConfig.Instance);
                },
                () => this.Helper.WriteConfig(ModConfig.Instance)
            )
        ).Register();
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        var config = ModConfig.Instance;

        if (config.OpenMenuByTelephone) return;

        if (config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else if (Context.IsPlayerFree)
                Game1.activeClickableMenu = new AMAMenu(config.DefaultMenuTabId, this.Helper);
        }
    }
}