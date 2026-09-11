using System;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.BetterCabin.UI;

/// <summary>
/// 一组按钮的动作开关：关掉时把每个按钮的点击动作临时置空并记住原值，打开时原样还原。
/// 面板按钮的误触守卫用它——宿主只在按钮的 <see cref="Button.OnClick" /> 非空时激活它并播确认音，
/// 置空即让手柄 A 既不触发动作也不播音（按钮的可见性、布局与焦点图都不动）。
/// </summary>
internal sealed class ButtonActionSwitch
{
    private readonly Button[] buttons;
    private readonly Action?[] actions;
    private bool enabled = true;

    /// <summary>构造动作开关（初始为打开：各按钮按各自已有的点击动作响应）。</summary>
    /// <param name="buttons">要统管点击动作的按钮。</param>
    public ButtonActionSwitch(params Button[] buttons)
    {
        this.buttons = buttons;

        // 原值在被关掉的那一刻才逐按钮记下（按钮的点击动作可能在开关构造之后才由宿主接线）
        this.actions = new Action?[buttons.Length];
    }

    /// <summary>true = 各按钮按各自的点击动作响应（初始状态）；false = 全部点击动作临时置空（原值记住供还原）。幂等。</summary>
    public bool IsEnabled
    {
        get => this.enabled;
        set
        {
            if (this.enabled == value)
            {
                return;
            }

            this.enabled = value;

            if (value)
            {
                this.RestoreActions();
            }
            else
            {
                this.SuspendActions();
            }
        }
    }

    /// <summary>把各按钮当前的点击动作收进原值表，再全部置空。</summary>
    private void SuspendActions()
    {
        for (var index = 0; index < this.buttons.Length; index++)
        {
            this.actions[index] = this.buttons[index].OnClick;
            this.buttons[index].OnClick = null;
        }
    }

    /// <summary>把原值表里的点击动作放回各按钮。</summary>
    private void RestoreActions()
    {
        for (var index = 0; index < this.buttons.Length; index++)
        {
            this.buttons[index].OnClick = this.actions[index];
        }
    }
}
