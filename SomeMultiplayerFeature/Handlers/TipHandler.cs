using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class TipHandler : BaseHandlerWithConfig<ModConfig>
{
    private readonly TextBox tipTextBox;

    public TipHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        this.tipTextBox = new TextBox(new Point(64, 144), config.TipText);
    }

    public override void Apply()
    {
        this.Helper.Events.Display.RenderingHud += this.OnRenderingHud;
    }

    public override void Clear()
    {
        this.Helper.Events.Display.RenderingHud -= this.OnRenderingHud;
    }

    private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!this.Config.ShowTip) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        this.tipTextBox.Draw(e.SpriteBatch);
    }
}