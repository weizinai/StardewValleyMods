using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class TipHandler : BaseHandler
{
    private readonly TextBox tipTextBox;

    public TipHandler(IModHelper helper) : base(helper)
    {
        this.tipTextBox = new TextBox(new Point(64, 144), ModConfig.Instance.TipText);
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
        if (ModConfig.Instance.ShowTip && Context.IsMultiplayer)
            this.tipTextBox.Draw(e.SpriteBatch);
    }
}