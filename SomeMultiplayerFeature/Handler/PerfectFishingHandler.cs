using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class PerfectFishingHandler : BaseHandler
{
    private const int RequiredPerfectCount = 3;

    private int perfectCount;

    public PerfectFishingHandler(IModHelper helper)
        : base(helper) { }

    public override void Apply()
    {
        this.helper.Events.Display.MenuChanged += this.OnMenuChanged;
    }

    public override void Clear()
    {
        this.helper.Events.Display.MenuChanged -= this.OnMenuChanged;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.OldMenu is BobberBar bar)
        {
            if (bar.perfect)
            {
                this.perfectCount++;

                if (this.perfectCount >= RequiredPerfectCount)
                {
                    Broadcaster<ModEntry>.NoIconHUDMessage($"{Game1.player.Name}连续3次完美钓鱼");
                    this.perfectCount = 0;
                }
            }
            else
                this.perfectCount = 0;
        }
    }
}
