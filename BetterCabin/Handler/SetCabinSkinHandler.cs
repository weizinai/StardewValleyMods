using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class SetCabinSkinHandler : BaseHandler
{
    public SetCabinSkinHandler(ModConfig config, IModHelper helper) : base(config, helper)
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
        if (Context.IsMainPlayer || !Context.IsPlayerFree) return;

        if (this.Config.SetCabinSkinKeybind.JustPressed())
        {
            Utility.ForEachBuilding(building =>
            {
                if (building.GetIndoors() is Cabin cabin && cabin.owner.Equals(Game1.player))
                {
                    Game1.activeClickableMenu = new BuildingSkinMenu(building, true);
                    return false;
                }

                return true;
            });
        }
    }
}