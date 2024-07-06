using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.MoreInfo.Framework;
using weizinai.StardewValleyMod.MoreInfo.Handler;

namespace weizinai.StardewValleyMod.MoreInfo;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private IInfoHandler[] handlers = Array.Empty<IInfoHandler>();
    private LocationInfoHandler[] locationInfoHandlers = Array.Empty<LocationInfoHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(this.Helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

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

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        var index = 0;
        foreach (var handler in this.locationInfoHandlers)
        {
            if (handler.IsEnable())
            {
                handler.Position = new Vector2(64 + 80 * index++, 0);
            }
        }
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
        this.locationInfoHandlers = this.handlers.OfType<LocationInfoHandler>().ToArray();

        foreach (var handler in this.handlers) handler.Init(this.Helper.Events);
    }

    private IEnumerable<IInfoHandler> GetHandlers()
    {
        if (this.config.ShowObjectInfo) yield return new ObjectInfoHandler();
        if (this.config.ShowMonsterInfo) yield return new MonsterInfoHandler();
        if (this.config.ShowBooksellerInfo) yield return new BooksellerInfoHandler();
    }
}