using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.AutoBreakGeode.Framework;
using weizinai.StardewValleyMod.AutoBreakGeode.Patcher;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.AutoBreakGeode;

internal class ModEntry : Mod
{
    public static bool AutoBreakGeode;
    private bool hasFastAnimation;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.hasFastAnimation = helper.ModRegistry.IsLoaded("Pathoschild.FastAnimations");
        I18n.Init(helper.Translation);
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供补丁程序读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        // 注册事件
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new GeodeMenuPatcher());
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModConfig.Instance.AutoBreakGeodeKeybind.JustPressed())
        {
            AutoBreakGeode = !AutoBreakGeode;
        }
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
                    {
                        for (var i = 0; i < ModConfig.Instance.BreakGeodeSpeed - 1; i++)
                        {
                            geodeMenu.update(Game1.currentGameTime);
                        }
                    }
                }

                if (Game1.player.freeSpotsInInventory() == 1)
                {
                    AutoBreakGeode = false;
                }
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

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddKeybindListOption(config => config.AutoBreakGeodeKeybind, I18n.Config_AutoBreakGeodeKeybind_Name)
            .AddBoolOption(config => config.DrawBeginButton, I18n.Config_DrawBeginButton_Name, I18n.Config_DrawBeginButton_Tooltip)
            .AddNumberOption(config => config.BreakGeodeSpeed, I18n.Config_BreakGeodeSpeed_Name);
    }
}
