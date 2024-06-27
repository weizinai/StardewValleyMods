using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class CabinMenuHandler : BaseHandler
{
    public CabinMenuHandler(ModConfig config, IModHelper helper) : base(config, helper)
    {
    }

    public override void Init()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        // if (Context.IsMainPlayer || !Context.IsPlayerFree) return;

        if (this.Config.CabinMenuKeybind.JustPressed())
        {
            var building = Game1.player.currentLocation.getBuildingAt(PositionHelper.GetTilePositionFromScreenPosition(
                new Vector2(Game1.getMouseX(false), Game1.getMouseY(false))));
            
            if (building.GetIndoors() is Cabin)
            {
                Game1.activeClickableMenu = new ClientCabinMenu(building);
            }
            
            // Utility.ForEachBuilding(building =>
            // {
            //     if (building.GetIndoors() is Cabin cabin && cabin.owner.Equals(Game1.player))
            //     {
            //         Game1.activeClickableMenu = new ClientCabinMenu(building);
            //         return false;
            //     }
            //
            //     return true;
            // });
        }
    }
}