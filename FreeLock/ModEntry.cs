using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.FreeLock.Framework;
using weizinai.StardewValleyMod.PiCore;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.FreeLock;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供事件处理器读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        // 注册事件
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Player.Warped += this.OnWarped;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || !Game1.viewportFreeze)
        {
            return;
        }

        PanScreenHelper.PanScreenByMouse(ModConfig.Instance.MoveSpeed, ModConfig.Instance.MoveThreshold);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree)
        {
            return;
        }

        if (ModConfig.Instance.FreeLockKeybind.JustPressed())
        {
            Game1.viewportFreeze = !Game1.viewportFreeze;
            HudLogger.NoIconHUDMessage(Game1.viewportFreeze ? I18n.UI_ViewportUnlocked_Tooltip() : I18n.UI_ViewportLocked_Tooltip(), 1000f);
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (Game1.viewportFreeze)
        {
            Game1.viewportFreeze = false;
            HudLogger.NoIconHUDMessage(I18n.UI_ViewportLocked_Tooltip(), 1000f);
        }
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddKeybindListOption(config => config.FreeLockKeybind, I18n.Config_FreeLockKeybind_Name)
            .AddNumberOption(config => config.MoveSpeed, I18n.Config_MoveSpeed_Name, I18n.Config_MoveSpeed_Tooltip)
            .AddNumberOption(config => config.MoveThreshold, I18n.Config_MoveThreshold_Name, I18n.Config_MoveThreshold_Tooltip);
    }
}
