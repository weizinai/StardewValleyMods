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
        this.Helper.Events.Display.RenderedHud += this.OnRenderedHud;
    }

    public override void Clear()
    {
        this.Helper.Events.Display.RenderedHud -= this.OnRenderedHud;
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (this.Config.ShowTip && Context.IsMultiplayer)
            this.tipTextBox.Draw(e.SpriteBatch);
    }
}