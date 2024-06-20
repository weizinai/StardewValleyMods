using Microsoft.Xna.Framework;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Framework.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class PlayerCountHandler : BaseHandler
{
    private readonly Button playerCountButton = new(new Point(64, 64), "");

    public PlayerCountHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        Helper.Events.GameLoop.OneSecondUpdateTicked += OnSecondUpdateTicked;
        Helper.Events.Display.RenderingHud += OnRenderingHud;
    }

    // 每秒检测当前在线玩家的数量
    private void OnSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!Config.ShowPlayerCount) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        playerCountButton.name = I18n.UI_PlayerCount(Game1.getOnlineFarmers().Count);
    }

    // 绘制玩家数量按钮
    private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!Config.ShowPlayerCount) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        playerCountButton.Draw(e.SpriteBatch);
    }
}