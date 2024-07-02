using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal class AutomationManger
{
    private ModConfig config;

    private bool modEnable = true;

    private int ticksPerAction;
    private int skippedActionTicks;

    public AutomationManger(IModHelper helper, ModConfig config)
    {
        // 初始化
        this.config = config;
        this.ticksPerAction = config.Cooldown;
        // 注册事件
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs updateTickedEventArgs)
    {
        if (!this.modEnable || !this.UpdateCooldown()) return;
        
    }

    private bool UpdateCooldown()
    {
        this.skippedActionTicks++;
        if (this.skippedActionTicks < this.ticksPerAction) return false;

        this.skippedActionTicks = 0;
        return true;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.config.ToggleModStateKeybind.JustPressed() && Context.IsPlayerFree)
        {
            var message = this.modEnable ? new HUDMessage(I18n.Message_ModDisable()) : new HUDMessage(I18n.Message_ModEnable());
            message.noIcon = true;
            Game1.addHUDMessage(message);
            this.modEnable = !this.modEnable;
        }
    }
}