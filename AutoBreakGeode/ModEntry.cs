using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.AutoBreakGeode.Framework;
using weizinai.StardewValleyMod.AutoBreakGeode.Patcher;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.AutoBreakGeode;

internal class ModEntry : Mod
{
    public static bool AutoBreakGeode;
    private ModConfig config = new();
    private bool hasFastAnimation;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.hasFastAnimation = helper.ModRegistry.IsLoaded("Pathoschild.FastAnimations");
        this.config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this.ModManifest.UniqueID, new GeodeMenuPatcher(this.config));
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.config.AutoBreakGeodeKeybind.JustPressed()) AutoBreakGeode = !AutoBreakGeode;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is GeodeMenu geodeMenu && AutoBreakGeode)
        {
            if (Utility.IsGeode(geodeMenu.heldItem))
            {
                if (geodeMenu.geodeAnimationTimer <= 0)
                {
                    var x = geodeMenu.geodeSpot.bounds.Center.X;
                    var y = geodeMenu.geodeSpot.bounds.Center.Y;
                    geodeMenu.receiveLeftClick(x, y);
                }
                else
                {
                    if (!this.hasFastAnimation)
                        for (var i = 0; i < this.config.BreakGeodeSpeed - 1; i++)
                            geodeMenu.update(Game1.currentGameTime);
                }

                if (Game1.player.freeSpotsInInventory() == 1) AutoBreakGeode = false;
            }
            else
            {
                AutoBreakGeode = false;
            }
        }
        else
        {
            AutoBreakGeode = false;
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForAutoBreakGeode(),
            () => this.config,
            _config => this.config = _config
        );
    }
}