using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Framework.Menu;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class CabinMenuHandler : BaseHandler
{
    public CabinMenuHandler(IModHelper helper) : base(helper) { }

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

        if (ModConfig.Instance.CabinMenuKeybind.JustPressed())
        {
            if (Context.IsMainPlayer)
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