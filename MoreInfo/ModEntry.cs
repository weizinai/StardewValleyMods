using Microsoft.Xna.Framework;
using MoreInfo.Framework;
using MoreInfo.Handler;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Integration;

namespace MoreInfo;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private IInfoHandler[] handlers = Array.Empty<IInfoHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(this.Helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

        helper.Events.Display.RenderingHud += this.RenderingHud;
        helper.Events.Display.RenderedHud += this.RenderedHud;

        helper.Events.Input.CursorMoved += this.OnCursorMoved;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForMoreInfo(
            new GenericModConfigMenuIntegration<ModConfig>(
                this.Helper.ModRegistry,
                this.ModManifest,
                () => this.config,
                () =>
                {
                    this.config = new ModConfig();
                    this.Helper.WriteConfig(this.config);
                    this.UpdateConfig();
                },
                () =>
                {
                    this.Helper.WriteConfig(this.config);
                    this.UpdateConfig();
                })
        ).Register();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.UpdateConfig();
    }

    private void RenderedHud(object? sender, RenderedHudEventArgs e)
    {
        foreach (var handler in this.handlers)
        {
            if (handler.IsEnable())
            {
                handler.DrawHoverText(e.SpriteBatch);
            }
        }
    }

    private void RenderingHud(object? sender, RenderingHudEventArgs e)
    {
        foreach (var handler in this.handlers)
        {
            if (handler.IsEnable())
            {
                handler.Draw(e.SpriteBatch);
            }
        }
    }

    private void OnCursorMoved(object? sender, CursorMovedEventArgs e)
    {
        foreach (var handler in this.handlers)
        {
            if (handler.IsEnable())
            {
                handler.UpdateHover(Utility.ModifyCoordinatesForUIScale(e.NewPosition.ScreenPixels));
            }
        }
    }

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers) handler.Clear(this.Helper.Events);

        this.handlers = this.GetHandlers().ToArray();

        foreach (var handler in this.handlers) handler.Init(this.Helper.Events);

        foreach (var handler in this.handlers)
        {
            handler.Position = new Vector2(0, Game1.uiViewport.Height / 2f - this.handlers.Length * 64 / 2f);
        }
    }

    private IEnumerable<IInfoHandler> GetHandlers()
    {
        if (this.config.ShowObjectInfo) yield return new ObjectInfoHandler();
    }
}