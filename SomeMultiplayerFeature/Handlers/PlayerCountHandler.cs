using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class PlayerCountHandler : BaseHandlerWithConfig<ModConfig>
{
    private readonly TextBox playerCountTextBox = new(new Point(64, 64), "");

    public PlayerCountHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

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
        if (this.Config.ShowPlayerCount && Context.IsMultiplayer)
            this.playerCountTextBox.name = I18n.UI_PlayerCount(Game1.getOnlineFarmers().Count);
    }

    // 绘制玩家数量按钮
    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (this.Config.ShowPlayerCount && Context.IsMultiplayer)
            this.playerCountTextBox.Draw(e.SpriteBatch);
    }
}