using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class PlayerCountHandler : BaseHandler
{
    private readonly TextBox playerCountTextBox = new(new Point(64, 64), "");

    public PlayerCountHandler(IModHelper helper)
        : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnSecondUpdateTicked;
        this.Helper.Events.Display.RenderedHud += this.OnRenderedHud;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnSecondUpdateTicked;
        this.Helper.Events.Display.RenderedHud -= this.OnRenderedHud;
    }

    // 每秒检测当前在线玩家的数量
    private void OnSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (ModConfig.Instance.ShowPlayerCount && Context.IsMultiplayer)
            this.playerCountTextBox.name = $"{Game1.getOnlineFarmers().Count}个玩家在线";
    }

    // 绘制玩家数量按钮
    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (ModConfig.Instance.ShowPlayerCount && Context.IsMultiplayer)
            this.playerCountTextBox.Draw(e.SpriteBatch);
    }
}