using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class IpConnectionHandler : BaseHandler
{
    public IpConnectionHandler(IModHelper helper)
        : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        this.Helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.DayStarted -= this.OnDayStarted;
        this.Helper.Events.GameLoop.TimeChanged -= this.OnTimeChanged;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!ModConfig.Instance.AutoSetIpConnection) return;

        if (Game1.IsClient) return;

        if (ModConfig.Instance.EnableTime == 6) this.EnableIpConnection();
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        // 如果该功能为启用，则返回
        if (!ModConfig.Instance.AutoSetIpConnection) return;

        // 如果当前不是多人模式或者当前玩家不是主玩家，则返回
        if (Game1.IsClient) return;

        if (Game1.timeOfDay == ModConfig.Instance.EnableTime * 100 && ModConfig.Instance.EnableTime != 6)
        {
            this.EnableIpConnection();
        }
        else if (Game1.timeOfDay == ModConfig.Instance.DisableTime * 100)
        {
            Logger.NoIconHUDMessage("Ip连接已关闭");
            Game1.options.ipConnectionsEnabled = false;
        }
    }

    private void EnableIpConnection()
    {
        if (Game1.isFestival())
        {
            Logger.NoIconHUDMessage("今天是节日，不打开Ip连接。");
            return;
        }
        Logger.NoIconHUDMessage("Ip连接已打开");
        Game1.options.ipConnectionsEnabled = true;
    }
}