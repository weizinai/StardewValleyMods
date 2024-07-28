using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class PerfectFishingHandler : BaseHandler
{
    private const int RequiredPerfectCount = 3;

    private int perfectCount;

    public PerfectFishingHandler(IModHelper helper)
        : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.Display.MenuChanged -= this.OnMenuChanged;
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
                    MultiplayerLog.NoIconHUDMessage($"{Game1.player.Name}连续3次完美钓鱼");
                    this.perfectCount = 0;
                }
            }
            else
            {
                this.perfectCount = 0;
            }
        }
    }
}