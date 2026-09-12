using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.FastControlInput.Framework;
using weizinai.StardewValleyMod.FastControlInput.Handler;
using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.FastControlInput;

internal class ModEntry : Mod
{
    private IInputHandler[] handlers = Array.Empty<IInputHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        var configService = new ConfigService<ModConfig>(this, this.UpdateConfig);
        configService.RegisterMenu(this.BuildConfigMenu);
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        foreach (var handler in this.handlers)
        {
            if (handler.IsEnable())
            {
                handler.Update();
            }
        }
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddNumberOption(
                config => config.ActionButton,
                I18n.Config_ActionButton_Name,
                I18n.Config_ActionButton_Tooltip,
                1f,
                10f,
                0.25f
            )
            .AddNumberOption(
                config => config.UseToolButton,
                I18n.Config_UseToolButton_Name,
                I18n.Config_UseToolButton_Tooltip,
                1f,
                10f,
                0.25f
            );
    }

    private void UpdateConfig()
    {
        this.handlers = this.GetHandlers().ToArray();
    }

    private IEnumerable<IInputHandler> GetHandlers()
    {
        if (ModConfig.Instance.ActionButton > 1)
        {
            yield return new ActionButtonHandler(ModConfig.Instance.ActionButton);
        }

        if (ModConfig.Instance.UseToolButton > 1)
        {
            yield return new UseToolButtonHandler(ModConfig.Instance.UseToolButton);
        }
    }
}
