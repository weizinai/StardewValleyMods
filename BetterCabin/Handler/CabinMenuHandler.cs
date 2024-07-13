using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Menu;
using weizinai.StardewValleyMod.Common.Extension;
using weizinai.StardewValleyMod.Common.Handler;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class CabinMenuHandler : BaseHandlerWithConfig<ModConfig>
{
    public CabinMenuHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.Config.CabinMenuKeybind.JustPressed())
        {
            if (Game1.IsServer)
            {
                Game1.activeClickableMenu = new ServerCabinMenu();
            }
            else
            {
                Utility.ForEachBuilding(building =>
                {
                    if (building.IsCabin(out var cabin) && cabin.owner.Equals(Game1.player))
                    {
                        Game1.activeClickableMenu = new ClientCabinMenu(building);
                        return false;
                    }

                    return true;
                });
            }
        }
    }
}