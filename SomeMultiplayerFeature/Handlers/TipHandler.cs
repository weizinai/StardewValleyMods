using Microsoft.Xna.Framework;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Framework.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SomeMultiplayerFeature.Handlers;

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
        Helper.Events.Display.Rendered += OnRendered;
    }

    private void OnRendered(object? sender, RenderedEventArgs e)
    {
        if (!Config.ShowTip) return;
        
        tipButton.Draw(e.SpriteBatch);
    }
}