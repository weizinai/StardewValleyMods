using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        : base(helper, config) {}

    public override void Init()
    {
        Helper.Events.GameLoop.OneSecondUpdateTicked += OnSecondUpdateTicked;
        Helper.Events.Display.Rendered += OnRendered;
    }

    // 每秒检测当前在线玩家的数量
    private void OnSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Config.ShowPlayerCount || !Context.IsWorldReady) return;
        playerCountButton.name = I18n.UI_PlayerCount(Game1.getOnlineFarmers().Count);
    }

    // 绘制玩家数量按钮
    private void OnRendered(object? sender, RenderedEventArgs e)
    {
        if (!Config.ShowPlayerCount || !Context.IsWorldReady) return;
        playerCountButton.Draw(e.SpriteBatch);
    }
}