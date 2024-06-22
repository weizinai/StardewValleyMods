using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework.UI;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class TipHandler : BaseHandler
{
    private readonly Button tipButton;

    public TipHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        tipButton = new Button(new Point(64, 144), config.TipText);
    }

    public override void Init()
    {
        Helper.Events.Display.RenderingHud += OnRenderingHud;
    }

    private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!Config.ShowTip) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        tipButton.Draw(e.SpriteBatch);
    }
}