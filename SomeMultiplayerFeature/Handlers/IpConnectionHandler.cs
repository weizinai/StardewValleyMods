using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class IpConnectionHandler : BaseHandlerWithConfig<ModConfig>
{
    public IpConnectionHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        this.Helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!this.Config.AutoSetIpConnection) return;

        if (Game1.IsClient) return;

        if (this.Config.EnableTime == 6) this.EnableIpConnection();
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        // 如果该功能为启用，则返回
        if (!this.Config.AutoSetIpConnection) return;

        // 如果当前不是多人模式或者当前玩家不是主玩家，则返回
        if (Game1.IsClient) return;

        if (Game1.timeOfDay == this.Config.EnableTime * 100)
        {
            this.EnableIpConnection();
        }
        else if (Game1.timeOfDay == this.Config.DisableTime * 100)
        {
            Log.NoIconHUDMessage("Ip连接已关闭");
            Game1.options.ipConnectionsEnabled = false;
        }
    }

    private void EnableIpConnection()
    {
        if (Game1.isFestival())
        {
            Log.NoIconHUDMessage("今天是节日，不打开Ip连接。");
            return;
        }
        Log.NoIconHUDMessage("Ip连接已打开");
        Game1.options.ipConnectionsEnabled = true;
    }
}